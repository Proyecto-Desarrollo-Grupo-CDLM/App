import { Component, inject, OnInit } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormControl } from '@angular/forms'; 
import { Router } from '@angular/router'; 
import { AuthService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared'; 

// --- IMPORTS DEL PROXY ---
import { CiudadService } from '../proxy/application/city-search';
import { 
  CiudadDto, 
  CitySearchRequestDto,
  CityDetailDto,       
  CityDetailRequestDto 
} from '../proxy/city-search';

// Importamos el servicio de Destinos y el DTO de creación
// Asegúrate de que la ruta sea correcta (ej. ../proxy/destinos o ../../proxy/destinos)
import { DestinoService, CreateUpdateDestinoDto } from '../proxy/destinos';

import {
  debounceTime,
  distinctUntilChanged,
  switchMap,
  catchError,
  Observable,
  of,
  tap, 
  BehaviorSubject, 
  combineLatest 
} from 'rxjs'; 

@Component({
  selector: 'app-ciudades',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule], 
  templateUrl: './ciudades.component.html',
  styleUrls: ['./ciudades.component.scss']
})
export class CiudadesComponent implements OnInit { 

  searchTerm = new FormControl(''); 
  ciudades$: Observable<CiudadDto[]> = of([]); 
  
  cargando: boolean = false;
  errorMessage: string | null = null; 
  isAuthError: boolean = false; 

  // --- VARIABLES PARA EL DETALLE ---
  expandedCityId: string | null = null;
  detailsCache: { [cityId: string]: CityDetailDto } = {}; 
  loadingDetail: boolean = false;
  detailError: string | null = null;

  // --- FILTROS ---
  selectedCountry: string = '';
  minPopulation: number | null = null;
  
  private filters$ = new BehaviorSubject<boolean>(true);

  countries: { code: string, name: string }[] = [
    { code: '', name: 'Todos los países' }
  ];

  private colorPalette: string[] = [
    '#0d6efd', '#ba94ebff', '#33d63bff', '#35d6dcff',
    '#fd7e14', '#198754', '#0dcaf0', '#212529'
  ];

  // Inyecciones
  private ciudadService = inject(CiudadService);
  private authService = inject(AuthService);
  private router = inject(Router);
  
  // Inyectamos los servicios necesarios para guardar
  private destinoService = inject(DestinoService);
  private toaster = inject(ToasterService);

  ngOnInit(): void {
    this.ciudades$ = combineLatest([
      this.searchTerm.valueChanges.pipe(debounceTime(400), distinctUntilChanged()),
      this.filters$ 
    ]).pipe(
      switchMap(([term, _]) => { 
        this.errorMessage = null;
        this.isAuthError = false;
        this.expandedCityId = null;

        if (!term || term.length < 3) {
          this.cargando = false; 
          return of([]); 
        }
        
        if (!this.authService.isAuthenticated) {
            this.cargando = false;
            this.isAuthError = true;
            this.errorMessage = '⚠️ Para buscar ciudades, necesitas iniciar sesión.';
            return of([]); 
        }

        this.cargando = true;

        const input: CitySearchRequestDto = { 
            nombreCiudad: term,
            countryCode: this.selectedCountry || undefined, 
            minPopulation: this.minPopulation || undefined
        };
        
        return this.ciudadService.searchCitiesByName(input).pipe(
          tap(() => {
            this.cargando = false;
          }),
          switchMap(response => {
             this.updateCountryFilters(response.cityNames);
             return of(response.cityNames);
          }), 
          catchError(err => {
            console.error('Error al buscar ciudades:', err);
            this.cargando = false;
            
            if (err.status === 401 || err.status === 403) {
              this.errorMessage = '⚠️ Tu sesión ha expirado. Por favor, inicia sesión nuevamente.';
              this.isAuthError = true; 
            } else {
              this.errorMessage = 'Ocurrió un error de conexión. Intente de nuevo.';
              this.isAuthError = false;
            }
                
            return of([]); 
          })
        );
      })
    );
    
    this.searchTerm.setValue(this.searchTerm.value || '');
  }

  toggleDetail(cityId: string): void {
    if (this.expandedCityId === cityId) {
      this.expandedCityId = null; 
      return;
    }

    this.expandedCityId = cityId;
    this.detailError = null;

    if (this.detailsCache[cityId]) {
      return; 
    }

    this.loadingDetail = true;
    const input: CityDetailRequestDto = { cityId: cityId };

    this.ciudadService.getCityDetailByInput(input).pipe(
        tap(() => this.loadingDetail = false),
        catchError(err => {
            console.error('Error obteniendo detalle:', err);
            this.loadingDetail = false;
            this.detailError = 'No se pudo cargar la información detallada.';
            return of(null);
        })
    ).subscribe((data) => {
        if (data) {
            this.detailsCache[cityId] = data;
        }
    });
  }

  private updateCountryFilters(ciudades: CiudadDto[]): void {
    if (this.selectedCountry) return;

    const uniqueCountries = new Map<string, string>();

    ciudades.forEach(c => {
      if (c.countryCode && c.pais) {
        uniqueCountries.set(c.countryCode, c.pais);
      }
    });

    if (uniqueCountries.size === 0) return;

    const newCountries = [
      { code: '', name: 'Todos los países' }
    ];

    uniqueCountries.forEach((name, code) => {
      newCountries.push({ code, name });
    });

    this.countries = newCountries.sort((a, b) => {
        if (a.code === '') return -1;
        return a.name.localeCompare(b.name);
    });
  }

  onFilterChange(): void {
    this.filters$.next(true);
  }

  redirectToLogin(): void {
    this.authService.navigateToLogin();
  }

  cancelAuthAction(): void { 
    this.errorMessage = null;
    this.isAuthError = false;
  }

  // <--- LÓGICA DE GUARDADO ADAPTADA (SOLUCIÓN A ERRORES ROJOS) ---
  // Actualiza tu método guardar en ciudades.component.ts
  guardar(ciudad: CiudadDto): void { 
    console.log('1. Iniciando guardado de:', ciudad.nombreCiudad);
    
    // Mostramos un aviso de "Cargando detalles..." porque esto tarda un milisegundo extra
    this.toaster.info('Obteniendo datos completos de la ciudad...', 'Procesando');

    // PASO 1: Pedimos el "Otro DTO" (CityDetailDto) que sí tiene la info
    this.ciudadService.getCityDetailByInput({ cityId: ciudad.id }).subscribe({
      next: (detalleCompleto) => {
        console.log('2. Detalles recibidos:', detalleCompleto);

        // PASO 2: Usamos el DTO DETALLADO para llenar los datos
        // Nota: Verifica si las propiedades de detalleCompleto vienen en mayúscula o minúscula en tu console.log
        const datosRicos = detalleCompleto as any; 

        const nuevoDestino: CreateUpdateDestinoDto = {
          nombre: ciudad.nombreCiudad, // El nombre de la lista suele estar bien
          pais: ciudad.pais || datosRicos.country || 'Desconocido',
          ciudad: ciudad.region || datosRicos.region || ciudad.nombreCiudad,
          
          // AQUÍ LA MAGIA: Usamos los datos del detalle
          poblacion: datosRicos.population || 0,
          
          // Coordenadas del detalle (Ajusta según si viene como objeto o propiedades sueltas)
          latitud: datosRicos.location?.latitude || datosRicos.latitude || 0,
          longitud: datosRicos.location?.longitude || datosRicos.longitude || 0,
          
          imageUrl: 'https://upload.wikimedia.org/wikipedia/commons/6/67/UTN_logo.jpg' // O datosRicos.imageUrl si la API lo trae
        };

        // PASO 3: Ahora sí, guardamos en la BD con todos los datos
        this.destinoService.create(nuevoDestino).subscribe({
          next: () => {
            this.toaster.success(`¡${ciudad.nombreCiudad} se guardó con sus datos completos!`, 'Éxito');
          },
          error: (err) => {
            console.error(err);
            // Si ya existe, mostramos error (validación que hicimos antes)
            this.toaster.error('No se pudo guardar (¿ya está en favoritos?).', 'Error');
          }
        });
      },
      error: (err) => {
        console.error('Error al obtener detalles:', err);
        this.toaster.error('No se pudieron obtener los detalles de la ciudad.', 'Error de API');
      }
    });
  }

  getColor(cityName: string): string { 
    if (!cityName) return this.colorPalette[0];
    let hash = 0;
    for (let i = 0; i < cityName.length; i++) {
      hash = cityName.charCodeAt(i) + ((hash << 5) - hash);
    }
    const index = Math.abs(hash) % this.colorPalette.length;
    return this.colorPalette[index]; 
  }
}