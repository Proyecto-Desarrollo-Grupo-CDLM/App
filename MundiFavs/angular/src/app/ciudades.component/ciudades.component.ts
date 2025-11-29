import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CiudadService } from '../proxy/application/city-search';
import { CiudadDto, CitySearchRequestDto } from '../proxy/city-search';

@Component({
  selector: 'app-ciudades',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ciudades.component.html',
  styleUrls: ['./ciudades.component.scss']
})
export class CiudadesComponent {
  
  ciudades: CiudadDto[] = [];
  textoBusqueda: string = '';
  cargando: boolean = false;

  // 🎨 PALETA DE COLORES (Colores oscuros para que el texto blanco se lea bien)
  private colorPalette: string[] = [
    '#0d6efd', // Azul
    '#ba94ebff', // Índigo
    '#33d63bff', // Rosa
    '#35d6dcff', // Rojo
    '#fd7e14', // Naranja
    '#198754', // Verde
    '#0dcaf0', // Cian
    '#212529'  // Negro suave
  ];

  private ciudadService = inject(CiudadService);

  buscar(): void {
    if (!this.textoBusqueda || this.textoBusqueda.length < 3) {
      return;
    }
    this.cargando = true;
    const input: CitySearchRequestDto = { nombreCiudad: this.textoBusqueda};
    this.ciudadService.searchCitiesByName(input).subscribe({
      next: (response) => {
        this.ciudades = response.cityNames;
        this.cargando = false;
      },
      error: (err) => {
        console.error(err);
        this.cargando = false;
      }
    });
  }

  guardar(ciudad: CiudadDto) {
    console.log('Guardando ciudad:', ciudad);
    alert(`¡${ciudad.nombreCiudad} guardada en favoritos! (Simulado)`);
  }

  // 🎨 FUNCIÓN MÁGICA: Genera un color consistente basado en el nombre
  getColor(cityName: string): string {
    if (!cityName) return this.colorPalette[0];
    let hash = 0;
    // Convierte el nombre en un número único
    for (let i = 0; i < cityName.length; i++) {
      hash = cityName.charCodeAt(i) + ((hash << 5) - hash);
    }
    // Usa ese número para elegir un color de la paleta
    const index = Math.abs(hash) % this.colorPalette.length;
    return this.colorPalette[index];
  }
}
