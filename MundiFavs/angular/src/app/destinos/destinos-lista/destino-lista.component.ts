
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PagedResultDto, CoreModule } from '@abp/ng.core';
import { DestinoService } from '../../proxy/destinos/destino.service';
import { DestinoDto, CreateUpdateDestinoDto } from '../../proxy/destinos/models';
import { finalize } from 'rxjs/operators';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { environment } from 'src/environments/environment';
import { DestinoSearchInputDto } from 'src/app/proxy/city-search';
import { CiudadDto } from 'src/app/proxy/city-search';
@Component({
  selector: 'app-destino-lista',
  standalone: true,
  imports: [CommonModule, FormsModule, CoreModule, NgbPaginationModule],
  templateUrl: './destino-lista.component.html',
  styleUrls: ['./destino-lista.component.scss'],
})
export class DestinoListaComponent implements OnInit {
  // Inyección de dependencias usando la nueva sintaxis de inject()
  private readonly destinoService = inject(DestinoService);

  /**
   * Lista de destinos obtenidos de la API
   */
  destinations: DestinoDto[] = [];

  /**
   * Indica si hay una petición en curso
   */
  loading = false;

  /**
   * Parámetros de búsqueda y paginación
   * - query: Término de búsqueda libre
   * - country: Filtro por país específico
   * - skipCount: Número de registros a saltar (para paginación)
   * - maxResultCount: Número máximo de registros por página
   */




  searchParams: DestinoSearchInputDto = {
    skipCount: 0,
    maxResultCount: 10,
    query: '',
    country: '',
  };
 

  /**
   * Total de registros disponibles (para paginación)
   */
  totalCount = 0;

  /**
   * Página actual (basada en 1)
   */
  currentPage = 1;

  /**
   * Imagen por defecto cuando el destino no tiene imageUrl
   */
  readonly defaultImage = 'assets/images/destination-placeholder.svg';

  ngOnInit(): void {
    // Cargar los destinos al inicializar el componente
    this.loadDestinations();
  }

  /**
   * Carga los destinos desde la API
   */
  private loadDestinations(): void {
    this.loading = true;

    this.destinoService
      .searchCities(this.searchParams)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: (result: PagedResultDto<DestinoDto>) => {
          // Asignar los resultados al array de destinos
          this.destinations = result.items || [];
          this.totalCount = result.totalCount || 0;
        },
        error: (error) => {
          // Manejar errores de la API
          console.error('Error al cargar destinos:', error);
          this.destinations = [];
          this.totalCount = 0;
        },
      });
  }

  /**
   * Maneja el evento de búsqueda
   * Reinicia la paginación y recarga los datos
   */
  onSearch(): void {
    // Reiniciar a la primera página
    this.searchParams.skipCount = 0;
    this.currentPage = 1;

    // Recargar los datos
    this.loadDestinations();
  }

  /**
   * Limpia los filtros de búsqueda y recarga todos los destinos
   */
  clearSearch(): void {
    this.searchParams.query = '';
    this.searchParams.country = '';
    this.onSearch();
  }

  /**
   * Maneja errores de carga de imágenes
   * Asigna la imagen por defecto cuando falla la carga
   *
   * @param event - Evento del error de imagen
   */
  onImageError(event: any): void {
    event.target.src = this.defaultImage;
  }

  /**
   * Formatea las coordenadas para mostrar
   *
   * @param latitude - Latitud del destino
   * @param longitude - Longitud del destino
   * @returns String con formato "lat, lng"
   */
  formatCoordinates(latitude: number, longitude: number): string {
    return `${latitude.toFixed(4)}, ${longitude.toFixed(4)}`;
  }

  /**
   * Formatea el número de población con separadores de miles
   *
   * @param population - Número de habitantes
   * @returns String formateado o mensaje si no hay datos
   */
  formatPopulation(population?: number): string {
    if (!population) {
      return 'N/A';
    }
    return population.toLocaleString('es-ES');
  }

  /**
   * Abre el destino en Google Maps usando las coordenadas
   *
   * @param destination - Destino turístico
   */
  openInMaps(destination: DestinoDto): void {
    const url = `https://www.google.com/maps/search/?api=1&query=${destination.ubicacion.latitud},${destination.ubicacion.longitud }`;
    window.open(url, '_blank');
  }

  /**
   * Maneja el cambio de página
   *
   * @param page - Número de la nueva página (basada en 1)
   */
  onPageChange(page: number): void {
    this.currentPage = page;
    // Calcular el skipCount basado en la página actual
    this.searchParams.skipCount = (page - 1) * this.searchParams.maxResultCount;
    this.loadDestinations();
  }

  /**
   * Devuelve la URL de la imagen de un destino.
   * @param imageUrl - URL relativa de la imagen
   */
  getDestinationImage(imageUrl: string): string {
    return imageUrl ? environment.apis.default.url + imageUrl : this.defaultImage;
  }
}
