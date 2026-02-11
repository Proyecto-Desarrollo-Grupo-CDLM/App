import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { CityDetailDto, CityDetailRequestDto, CitySearchRequestDto, CitySearchResultDto } from '../../city-search/models';

@Injectable({
  providedIn: 'root',
})
export class CiudadService {
  apiName = 'Default';
  

  getCityDetailByInput = (input: CityDetailRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CityDetailDto>({
      method: 'GET',
      url: '/api/app/ciudad/city-detail',
      params: { cityId: input.cityId },
    },
    { apiName: this.apiName,...config });
  

  searchCitiesByName = (request: CitySearchRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CitySearchResultDto>({
      method: 'POST',
      url: '/api/app/ciudad/search-cities-by-name',
      body: request,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
