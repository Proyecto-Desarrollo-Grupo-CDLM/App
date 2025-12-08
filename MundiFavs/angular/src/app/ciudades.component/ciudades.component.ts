import { Component, inject, OnInit } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormControl } from '@angular/forms'; 
import { Router } from '@angular/router'; 
import { CiudadService } from '../proxy/application/city-search';
import { CiudadDto, CitySearchRequestDto } from '../proxy/city-search';

import {
  debounceTime,
  distinctUntilChanged,
  switchMap,
  catchError,
  Observable,
  of,
  tap, // 👈 Añadido 'tap'
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
    '#0d6efd',
    '#ba94ebff',
    '#33d63bff',
    '#35d6dcff',
    '#fd7e14',
    '#198754',
    '#0dcaf0',
    '#212529'
  ];

  private ciudadService = inject(CiudadService);
  private router = inject(Router); 

  ngOnInit(): void {
    this.ciudades$ = this.searchTerm.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged(),
      switchMap(term => {
        if (!term || term.length < 3) {
          this.cargando = false; 
          this.errorMessage = null;
          this.isAuthError = false; 
          return of([]); 
        }
        
        this.cargando = true;
        this.errorMessage = null;
        this.isAuthError = false;

        const input: CitySearchRequestDto = { nombreCiudad: term };
        
        return this.ciudadService.searchCitiesByName(input).pipe(
          tap(() => {
            // Utilizamos tap para manejar el efecto secundario de ocultar el loading
            this.cargando = false;
          }),
          switchMap(response => of(response.cityNames)), // Extraemos el array y lo devolvemos

          catchError(err => {
            console.error('Error al buscar ciudades (API):', err);
            this.cargando = false;
            
            if (err.status === 401 || err.status === 403) {
              this.errorMessage = '⚠️ Se requiere iniciar sesión para utilizar esta funcionalidad.';
              this.isAuthError = true; // Habilita la vista de los botones de acción
            } else {
              this.errorMessage = 'Ocurrió un error al buscar las ciudades. Intente de nuevo.';
              this.isAuthError = false;
            }
                
            return of([]); // Devolver un observable vacío en caso de error
          })
        );
      })
    );
  }

  redirectToLogin(): void {
    // Redirige al login, como lo requiere el flujo del TP7/TP8 [cite: 343]
    this.router.navigate(['/account/login']); 
  }

  cancelAuthAction(): void { 
    // Limpia el estado de error para permitir al usuario continuar
    this.errorMessage = null;
    this.isAuthError = false;
    this.searchTerm.setValue(''); // Opcional: limpiar la búsqueda
  }

  guardar(ciudad: CiudadDto): void { 
    console.log('Guardando ciudad:', ciudad);
    // Esto está simulado. En el TP5 se implementó guardar el destino en la base interna.
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