import { Component, inject, OnInit } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormControl } from '@angular/forms'; 
import { Router } from '@angular/router'; 
import { CiudadService } from '../proxy/application/city-search';
import { CiudadDto, CitySearchRequestDto } from '../proxy/city-search';
// 1. Importar AuthService
import { AuthService } from '@abp/ng.core';

import {
  debounceTime,
  distinctUntilChanged,
  switchMap,
  catchError,
  Observable,
  of,
  tap, 
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

  private colorPalette: string[] = [
    '#0d6efd', '#ba94ebff', '#33d63bff', '#35d6dcff',
    '#fd7e14', '#198754', '#0dcaf0', '#212529'
  ];

  private ciudadService = inject(CiudadService);
  // 2. Inyectar AuthService
  private authService = inject(AuthService);

  ngOnInit(): void {
    this.ciudades$ = this.searchTerm.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged(),
      switchMap(term => {
        // Limpieza inicial
        this.errorMessage = null;
        this.isAuthError = false;

        if (!term || term.length < 3) {
          this.cargando = false; 
          return of([]); 
        }
        
        // 3. VERIFICACIÓN PROACTIVA DE SESIÓN
        // Si no está logueado, mostramos el error y cancelamos la búsqueda inmediatamente.
        if (!this.authService.isAuthenticated) {
            this.cargando = false;
            this.isAuthError = true;
            this.errorMessage = '⚠️ Para buscar ciudades, necesitas iniciar sesión.';
            return of([]); // Retornamos vacío para no llamar a la API
        }

        // Si pasa la verificación, procedemos con la carga
        this.cargando = true;

        const input: CitySearchRequestDto = { nombreCiudad: term };
        
        return this.ciudadService.searchCitiesByName(input).pipe(
          tap(() => {
            this.cargando = false;
          }),
          switchMap(response => of(response.cityNames)), 

          catchError(err => {
            console.error('Error al buscar ciudades (API):', err);
            this.cargando = false;
            
            // Mantenemos esto por seguridad, por si la sesión expira mientras navega
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
  }

  redirectToLogin(): void {
    // 4. Usar la redirección nativa de ABP
    // Esto es mejor porque guarda la URL actual y te devuelve aquí después del login.
    this.authService.navigateToLogin();
  }

  cancelAuthAction(): void { 
    this.errorMessage = null;
    this.isAuthError = false;
    // Opcional: Borrar el texto para reiniciar
    // this.searchTerm.setValue('', { emitEvent: false }); 
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