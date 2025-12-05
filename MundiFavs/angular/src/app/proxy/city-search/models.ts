
export interface CitySearchRequestDto {
  nombreCiudad?: string;
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
