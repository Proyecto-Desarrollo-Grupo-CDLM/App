import { Component, inject, OnInit } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormControl } from '@angular/forms'; 
import { Router } from '@angular/router'; 
import { AuthService } from '@abp/ng.core';

// --- IMPORTS DEL PROXY (Generados automáticamente) ---
// Verifica que la ruta '../proxy/city-search' sea correcta en tu carpeta
import { CiudadService } from '../proxy/application/city-search';
import { 
  CiudadDto, 
  CitySearchRequestDto,
  CityDetailDto,       
  CityDetailRequestDto 
} from '../proxy/city-search';

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
  // Cache usando el tipo real 'CityDetailDto'
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

  private ciudadService = inject(CiudadService);
  private authService = inject(AuthService);
  private router = inject(Router);

  ngOnInit(): void {
    this.ciudades$ = combineLatest([
      this.searchTerm.valueChanges.pipe(debounceTime(400), distinctUntilChanged()),
      this.filters$ 
    ]).pipe(
      switchMap(([term, _]) => { 
        this.errorMessage = null;
        this.isAuthError = false;
        this.expandedCityId = null; // Reset al buscar

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

  // --- LÓGICA DEL DETALLE ---
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

    // Usamos el DTO oficial del proxy
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

  guardar(ciudad: CiudadDto): void { 
    console.log('Guardando ciudad:', ciudad);
    alert(`¡${ciudad.nombreCiudad} guardada en favoritos! (Simulado)`);
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