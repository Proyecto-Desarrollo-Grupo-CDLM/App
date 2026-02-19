import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { CityDetailDto, CityDetailRequestDto, CitySearchRequestDto, CitySearchResultDto } from '../../city-search/models';

@Injectable({
  providedIn: 'root',
})
export class GeoDbCitySearchService {
  apiName = 'Default';
  

  getCityDetailByIdByInput = (input: CityDetailRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CityDetailDto>({
      method: 'GET',
      url: '/api/app/geo-db-city-search/city-detail-by-id',
      params: { cityId: input.cityId },
    },
    { apiName: this.apiName,...config });
  

  searchCities = (request: CitySearchRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CitySearchResultDto>({
      method: 'POST',
      url: '/api/app/geo-db-city-search/search-cities',
      body: request,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
