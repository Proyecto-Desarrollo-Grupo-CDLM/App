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

import { DestinoService, CreateUpdateDestinoDto, ComentarioDto } from '../proxy/destinos';

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

  // --- BÚSQUEDA Y LISTADO ---
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

  // 🔹 COMENTARIOS Y CALIFICACIÓN (PROPIEDADES QUE FALTABAN)
  comentarios: ComentarioDto[] = [];
  promedioEstrellas: number = 0;
  totalCalificaciones: number = 0;
  nombreDestino: string = '';
  loadingComments: boolean = false;
  comentariosError: string | null = null;
  
  // Cache para comentarios por ciudad (para no recargar innecesariamente)
  comentariosCache: { [cityId: string]: { 
    comentarios: ComentarioDto[], 
    promedio: number, 
    total: number, 
    nombre: string 
  }} = {};

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
            console.error('Error en búsqueda de ciudades:', err);
            return of([]); 
          })
        );
      })
    );
    this.searchTerm.setValue(this.searchTerm.value || '');
  }

  /**
   * 🔹 Alterna el detalle de una ciudad y carga info/comentarios según la pestaña
   */
  toggleDetail(cityId: string, tab: 'info' | 'comentarios' = 'info'): void {
    // Si ya está expandida la misma ciudad en la misma pestaña, colapsar
    if (this.expandedCityId === cityId && this.activeTab[cityId] === tab) {
      this.expandedCityId = null; 
      this.resetComentarios(); // 🔹 Limpiar comentarios al cerrar
      return;
    }

    // Expandir nueva ciudad
    this.expandedCityId = cityId;
    this.activeTab[cityId] = tab;
    this.detailError = null;
    this.comentariosError = null;
    this.filtroEstrellas = null;

    // 🔹 Si la pestaña es "info", cargar detalles de GeoDB
    if (tab === 'info') {
      this.loadCityDetails(cityId);
    } 
    // 🔹 Si la pestaña es "comentarios", cargar comentarios de BD interna
    else if (tab === 'comentarios') {
      this.loadComentarios(cityId);
    }
  }

  /**
   * 🔹 Carga detalles de GeoDB (población, coordenadas, etc.)
   */
  private loadCityDetails(cityId: string): void {
    // Si ya tenemos la info en caché, no recargar
    if (this.detailsCache[cityId]) {
      return;
    }

    this.loadingDetail = true;
    this.ciudadService.getCityDetailByInput({ cityId: cityId }).pipe(
        tap(() => this.loadingDetail = false),
        catchError(err => {
            this.loadingDetail = false;
            this.detailError = 'No se pudo cargar la información de GeoDB.';
            console.error('Error cargando detalles:', err);
            return of(null);
        })
    ).subscribe((data) => {
        if (data) {
          this.detailsCache[cityId] = data;
        }
    });
  }

private loadComentarios(cityId: string): void {
  // Verificar si ya tenemos los comentarios en caché
  if (this.comentariosCache[cityId]) {
    const cached = this.comentariosCache[cityId];
    this.comentarios = cached.comentarios;
    this.promedioEstrellas = cached.promedio;
    this.totalCalificaciones = cached.total;
    this.nombreDestino = cached.nombre;
    return;
  }

  // Resetear estado antes de cargar
  this.resetComentarios();
  this.loadingComments = true;
  this.comentariosError = null;

  // 🔹 CRÍTICO: Convertir a string y limpiar
 const externalIdLimpio = String(cityId);
  console.log('Buscando con ID:', externalIdLimpio)

  // Llamar al backend con el externalId
  this.destinoService.getComentariosConPromedio(externalIdLimpio).subscribe({
    next: (result) => {
      console.log('✅ Comentarios recibidos:', result); // DEBUG
      
      this.comentarios = result.comentarios || [];
      this.promedioEstrellas = result.puntuacionPromedio || 0;
      this.totalCalificaciones = result.totalCalificaciones || 0;
      this.nombreDestino = result.nombreDestino || 'Destino';
      this.loadingComments = false;

      // Guardar en caché
      this.comentariosCache[cityId] = {
        comentarios: this.comentarios,
        promedio: this.promedioEstrellas,
        total: this.totalCalificaciones,
        nombre: this.nombreDestino
      };
    },
    error: (err) => {
      console.error('❌ Error cargando comentarios:', err); // DEBUG
      console.error('❌ ExternalId usado:', externalIdLimpio); // DEBUG
      
      this.loadingComments = false;
      this.comentariosError = 'No se pudieron cargar los comentarios.';
    }
  });
}

  /**
   * 🔹 Resetea las propiedades de comentarios
   */
  private resetComentarios(): void {
    this.comentarios = [];
    this.promedioEstrellas = 0;
    this.totalCalificaciones = 0;
    this.nombreDestino = '';
    this.comentariosError = null;
  }

  /**
   * 🔹 Filtra comentarios por cantidad de estrellas
   */
  getComentariosFiltrados(): ComentarioDto[] {
    if (this.filtroEstrellas === null) {
      return this.comentarios;
    }
    return this.comentarios.filter(c => c.estrellas === this.filtroEstrellas);
  }

  /**
   * 🔹 Aplicar filtro de estrellas
   */
  aplicarFiltroEstrellas(estrellas: number | null): void {
    this.filtroEstrellas = estrellas;
  }

  /**
   * Actualiza los filtros de países disponibles
   */
  private updateCountryFilters(ciudades: CiudadDto[]): void {
    if (this.selectedCountry) return;
    const uniqueCountries = new Map<string, string>();
    ciudades.forEach(c => { 
      if (c.countryCode && c.pais) {
        uniqueCountries.set(c.countryCode, c.pais); 
      }
    });
    const newCountries = [{ code: '', name: 'Todos los países' }];
    uniqueCountries.forEach((name, code) => newCountries.push({ code, name }));
    this.countries = newCountries.sort((a, b) => 
      a.code === '' ? -1 : a.name.localeCompare(b.name)
    );
  }

  /**
   * Dispara recarga de búsqueda cuando cambian filtros
   */
  onFilterChange(): void { 
    this.filters$.next(true); 
  }

  /**
   * Redirige al login
   */
  redirectToLogin(): void { 
    this.authService.navigateToLogin(); 
  }

  /**
   * Cancela acción de autenticación
   */
  cancelAuthAction(): void { 
    this.errorMessage = null; 
    this.isAuthError = false; 
  }

  /**
   * 🔹 Guarda un destino en la BD interna y limpia caché de comentarios
   */
 guardar(ciudad: CiudadDto): void { 
  this.toaster.info('Obteniendo datos completos...', 'Procesando');
  
  this.ciudadService.getCityDetailByInput({ cityId: ciudad.id }).subscribe({
    next: (detalleCompleto) => {
      const datosRicos = detalleCompleto as any; 
      
      // 🔹 CRÍTICO: Asegurarse de que el externalId sea string y esté limpio
      const idSeguro = String(ciudad.id).trim();
      
      console.log('💾 Guardando destino con externalId:', idSeguro); // DEBUG
      
      const nuevoDestino: CreateUpdateDestinoDto = {
        nombre: ciudad.nombreCiudad,
        pais: ciudad.pais || datosRicos.country || 'Desconocido',
        ciudad: ciudad.region || datosRicos.region || ciudad.nombreCiudad,
        poblacion: datosRicos.population || 0,
        latitud: datosRicos.location?.latitude || datosRicos.latitude || 0,
        longitud: datosRicos.location?.longitude || datosRicos.longitude || 0,
        imageUrl: 'https://upload.wikimedia.org/wikipedia/commons/6/67/UTN_logo.jpg',
        externalId: idSeguro // 
      };
      
      this.destinoService.create(nuevoDestino).subscribe({
        next: () => {
          this.toaster.success(`¡${ciudad.nombreCiudad} se guardó con éxito!`, 'Éxito');
          
          // Limpiar cachés
          delete this.detailsCache[ciudad.id];
          delete this.comentariosCache[ciudad.id];
          
          // Si estamos viendo comentarios, recargar
          if (this.expandedCityId === ciudad.id && this.activeTab[ciudad.id] === 'comentarios') {
            this.loadComentarios(ciudad.id);
          }
        },
        error: (err) => {
          console.error('❌ Error guardando destino:', err);
          this.toaster.error('Error al guardar el destino.', 'Error');
        }
      });
    },
    error: (err) => {
      console.error('❌ Error obteniendo detalles:', err);
      this.toaster.error('Error al obtener datos completos.', 'Error');
    }
  });
}

  /**
   * Genera un color basado en el nombre de la ciudad
   */
  getColor(cityName: string): string { 
    if (!cityName) return this.colorPalette[0];
    let hash = 0;
    for (let i = 0; i < cityName.length; i++) {
      hash = cityName.charCodeAt(i) + ((hash << 5) - hash);
    }
    return this.colorPalette[Math.abs(hash) % this.colorPalette.length]; 
  }

  /**
   * 🔹 Métodos auxiliares para las estrellas en el template
   */
  getEstrellas(cantidad: number): number[] {
    const estrellasLlenas = Math.floor(cantidad);
    return Array(estrellasLlenas).fill(0);
  }

  tieneMediaEstrella(cantidad: number): boolean {
    return (cantidad % 1) >= 0.5;
  }

  getEstrellasVacias(cantidad: number): number[] {
    const estrellasLlenas = Math.ceil(cantidad);
    const vacias = 5 - estrellasLlenas;
    return Array(vacias > 0 ? vacias : 0).fill(0);
  }

  /**
   * 🔹 Método helper para acceder a Math en el template
   */
  get Math() {
    return Math;
  }
}