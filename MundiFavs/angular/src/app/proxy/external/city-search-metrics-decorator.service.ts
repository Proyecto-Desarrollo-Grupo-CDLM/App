import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { CityDetailDto, CityDetailRequestDto, CitySearchRequestDto, CitySearchResultDto } from '../city-search/models';

@Injectable({
  providedIn: 'root',
})
export class CitySearchMetricsDecoratorService {
  apiName = 'Default';
  

  getCityDetailByIdByInput = (input: CityDetailRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CityDetailDto>({
      method: 'GET',
      url: '/api/app/city-search-metrics-decorator/city-detail-by-id',
      params: { cityId: input.cityId },
    },
    { apiName: this.apiName,...config });
  

  searchCities = (input: CitySearchRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CitySearchResultDto>({
      method: 'POST',
      url: '/api/app/city-search-metrics-decorator/search-cities',
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
