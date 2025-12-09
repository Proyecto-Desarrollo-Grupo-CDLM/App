
export interface CitySearchRequestDto {
  nombreCiudad: string;
  countryCode?: string;
  minPopulation?: number;
}

export interface CitySearchResultDto {
  cityNames: CiudadDto[];
}

export interface CiudadDto {
  nombreCiudad?: string;
  pais?: string;
  region?: string;
  id?: string;
  countryCode?: string;
}
