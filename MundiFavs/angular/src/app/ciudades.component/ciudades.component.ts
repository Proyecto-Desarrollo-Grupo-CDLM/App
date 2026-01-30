import { Component, inject, OnInit } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormControl } from '@angular/forms'; 
import { Router } from '@angular/router'; 
import { AuthService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared'; 

import { CiudadService } from '../proxy/application/city-search';
import { 
  CiudadDto, 
  CitySearchRequestDto,
  CityDetailDto,       
  CityDetailRequestDto 
} from '../proxy/city-search';

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

  // --- VARIABLES PARA EL DETALLE Y PESTAÑAS ---
  expandedCityId: string | null = null;
  detailsCache: { [cityId: string]: CityDetailDto } = {}; 
  loadingDetail: boolean = false;
  detailError: string | null = null;

  // Control de pestañas por cada ciudad
  activeTab: { [cityId: string]: 'info' | 'comentarios' } = {};
  filtroEstrellas: number | null = null;

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

  private ciudadService = inject(CiudadService);
  private authService = inject(AuthService);
  private router = inject(Router);
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
          tap(() => this.cargando = false),
          switchMap(response => {
             this.updateCountryFilters(response.cityNames);
             return of(response.cityNames);
          }), 
          catchError(err => {
            this.cargando = false;
            this.errorMessage = 'Ocurrió un error de conexión. Intente de nuevo.';
            return of([]); 
          })
        );
      })
    );
    this.searchTerm.setValue(this.searchTerm.value || '');
  }

  // Ahora acepta la pestaña deseada ('info' o 'comentarios')
  toggleDetail(cityId: string, tab: 'info' | 'comentarios' = 'info'): void {
    if (this.expandedCityId === cityId && this.activeTab[cityId] === tab) {
      this.expandedCityId = null; 
      return;
    }

    // Si queremos comentarios, borramos caché para forzar actualización de BD
    if (tab === 'comentarios') {
      delete this.detailsCache[cityId];
    }

    this.expandedCityId = cityId;
    this.activeTab[cityId] = tab;
    this.detailError = null;
    this.filtroEstrellas = null;

    if (this.detailsCache[cityId]) return; 

    this.loadingDetail = true;
    this.ciudadService.getCityDetailByInput({ cityId: cityId }).pipe(
        tap(() => this.loadingDetail = false),
        catchError(err => {
            this.loadingDetail = false;
            this.detailError = 'No se pudo cargar la información.';
            return of(null);
        })
    ).subscribe((data) => {
        if (data) this.detailsCache[cityId] = data;
    });
  }

  // Lógica para filtrar los comentarios que vienen en el detalle
  getCalificacionesFiltradas(cityId: string): any[] {
    const detalle = this.detailsCache[cityId] as any;
    const lista = detalle?.calificaciones || [];
    
    if (this.filtroEstrellas === null) return lista;
    return lista.filter(c => c.estrellas === this.filtroEstrellas);
  }

  private updateCountryFilters(ciudades: CiudadDto[]): void {
    if (this.selectedCountry) return;
    const uniqueCountries = new Map<string, string>();
    ciudades.forEach(c => { if (c.countryCode && c.pais) uniqueCountries.set(c.countryCode, c.pais); });
    const newCountries = [{ code: '', name: 'Todos los países' }];
    uniqueCountries.forEach((name, code) => newCountries.push({ code, name }));
    this.countries = newCountries.sort((a, b) => a.code === '' ? -1 : a.name.localeCompare(b.name));
  }

  onFilterChange(): void { this.filters$.next(true); }
  redirectToLogin(): void { this.authService.navigateToLogin(); }
  cancelAuthAction(): void { this.errorMessage = null; this.isAuthError = false; }

  guardar(ciudad: CiudadDto): void { 
    this.toaster.info('Obteniendo datos completos...', 'Procesando');
    this.ciudadService.getCityDetailByInput({ cityId: ciudad.id }).subscribe({
      next: (detalleCompleto) => {
        const datosRicos = detalleCompleto as any; 
        const nuevoDestino: CreateUpdateDestinoDto = {
          nombre: ciudad.nombreCiudad,
          pais: ciudad.pais || datosRicos.country || 'Desconocido',
          ciudad: ciudad.region || datosRicos.region || ciudad.nombreCiudad,
          poblacion: datosRicos.population || 0,
          latitud: datosRicos.location?.latitude || datosRicos.latitude || 0,
          longitud: datosRicos.location?.longitude || datosRicos.longitude || 0,
          imageUrl: 'https://upload.wikimedia.org/wikipedia/commons/6/67/UTN_logo.jpg'
        };
        this.destinoService.create(nuevoDestino).subscribe({
          next: () => {
            this.toaster.success(`¡Se guardó con éxito!`, 'Éxito');
            // Limpiamos caché para que al abrir comentarios se vea la nueva info
            delete this.detailsCache[ciudad.id];
          },
          error: () => this.toaster.error('Error al guardar.', 'Error')
        });
      }
    });
  }

  getColor(cityName: string): string { 
    if (!cityName) return this.colorPalette[0];
    let hash = 0;
    for (let i = 0; i < cityName.length; i++) hash = cityName.charCodeAt(i) + ((hash << 5) - hash);
    return this.colorPalette[Math.abs(hash) % this.colorPalette.length]; 
  }
}