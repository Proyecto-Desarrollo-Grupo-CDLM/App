import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// Importa el SERVICIO desde la carpeta 'application/city-search'
import { CiudadService } from '../proxy/application/city-search'; 

// Importa los MODELOS (DTOs) desde la carpeta 'city-search'
import { CiudadDto, CitySearchRequestDto } from '../proxy/city-search';

@Component({
  selector: 'app-ciudades',
  standalone: true, 
  imports: [CommonModule, FormsModule], 
  templateUrl: './ciudades.component.html',
  styleUrls: ['./ciudades.component.scss'] 
})


export class CiudadesComponent { 

  // 1. Variables para la vista
  ciudades: CiudadDto[] = []; // La lista de resultados
  textoBusqueda: string = ''; // Lo que escribe el usuario

  cargando: boolean = false;

  // 2. Inyectamos el servicio de ABP (Backend)
  private readonly ciudadService = inject(CiudadService);

  // 3. Método para buscar (reemplaza al create/delete del ejemplo)
  buscar(): void {
    // Validamos que no esté vacío
    if (!this.textoBusqueda || this.textoBusqueda.length < 3) {
      return; 
    }

    this.cargando = true;

    // Creamos el DTO de entrada que pide el backend
    const input: CitySearchRequestDto = {
      nombreCiudad: this.textoBusqueda
    };

    // Llamamos al servicio
    this.ciudadService.searchCitiesByName(input).subscribe({
      next: (response) => {
        this.ciudades = response.cityNames;
        this.cargando = false;
      },
      error: (err) => {
        console.error('Error', err);
        this.cargando = false;
      }
    });
  }
}
