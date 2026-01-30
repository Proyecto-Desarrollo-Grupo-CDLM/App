
export interface CityDetailDto {
  id?: string;
  nombreCiudad?: string;
  pais?: string;
  region?: string;
  latitud: number;
  longitud: number;
  poblacion: number;
  utcOffset?: string;
}

export interface CityDetailRequestDto {
  cityId?: string;
}

export interface CitySearchRequestDto {
  nombreCiudad?: string;
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
