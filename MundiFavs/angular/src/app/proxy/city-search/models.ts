
export interface CitySearchRequestDto {
  nombreCiudad?: string;
}

export interface DestinoSearchInputDto {
    nombreCiudad?: string; // Si usas 'query' abajo, quizás este sobre o sea el mismo.
    query?: string;        // El texto que busca el usuario
    country?: string;      // Filtro por país
    skipCount?: number;    // Para paginación (ABP lo usa mucho)
    maxResultCount?: number; // Cantidad de items por página
}

export interface CitySearchResultDto {
  cityNames: CiudadDto[];
}

export interface CiudadDto {
  nombreCiudad?: string;
  pais?: string;
  region?: string;
  id?: string;
}
