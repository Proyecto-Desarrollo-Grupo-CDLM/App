import { Component, inject, OnInit } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormControl } from '@angular/forms'; 
import { Router } from '@angular/router'; 
import { CiudadService } from '../proxy/application/city-search';
import { CiudadDto, CitySearchRequestDto } from '../proxy/city-search';
import { AuthService } from '@abp/ng.core';

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

  // --- FILTROS ---
  selectedCountry: string = '';
  minPopulation: number | null = null;
  
  private filters$ = new BehaviorSubject<boolean>(true);

  // Lista de países inicial (se actualizará dinámicamente)
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
          // --- AQUÍ ACTUALIZAMOS LOS FILTROS ---
          switchMap(response => {
             // Llamamos al método para actualizar el combo de países con los resultados
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

  // --- MÉTODO NUEVO PARA FILTROS DINÁMICOS ---
  private updateCountryFilters(ciudades: CiudadDto[]): void {
    // Si el usuario ya filtró por un país, no cambiamos la lista para no confundirlo
    if (this.selectedCountry) return;

    const uniqueCountries = new Map<string, string>();

    ciudades.forEach(c => {
      // Usamos el nuevo campo countryCode si existe, o tratamos de inferirlo si no
      if (c.countryCode && c.pais) {
        uniqueCountries.set(c.countryCode, c.pais);
      }
    });

    // Si no encontramos códigos (porque el backend no los mandó aún), no hacemos nada
    if (uniqueCountries.size === 0) return;

    const newCountries = [
      { code: '', name: 'Todos los países' }
    ];

    uniqueCountries.forEach((name, code) => {
      newCountries.push({ code, name });
    });

    // Ordenamos alfabéticamente
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