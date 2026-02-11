import type { ComentarioDto, CreateUpdateDestinoDto, DestinoComentariosDto, DestinoDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { CitySearchRequestDto, CitySearchResultDto } from '../city-search/models';

@Injectable({
  providedIn: 'root',
})
export class DestinoService {
  apiName = 'Default';
  

  create = (input: CreateUpdateDestinoDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoDto>({
      method: 'POST',
      url: '/api/app/destino',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/destino/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoDto>({
      method: 'GET',
      url: `/api/app/destino/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getComentariosConPromedio = (externalCityId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoComentariosDto>({
      method: 'GET',
      url: `/api/app/destino/comentarios-con-promedio/${externalCityId}`,
    },
    { apiName: this.apiName,...config });
  

  getComentariosPorIdExterno = (externalCityId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ComentarioDto[]>({
      method: 'GET',
      url: `/api/app/destino/comentarios-por-id-externo/${externalCityId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DestinoDto>>({
      method: 'GET',
      url: '/api/app/destino',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getMyDestinations = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DestinoDto>>({
      method: 'GET',
      url: '/api/app/destino/my-destinations',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getPopularDestinations = (maxCount: number = 10, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoDto[]>({
      method: 'GET',
      url: '/api/app/destino/popular-destinations',
      params: { maxCount },
    },
    { apiName: this.apiName,...config });
  

  searchCities = (request: CitySearchRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CitySearchResultDto>({
      method: 'POST',
      url: '/api/app/destino/search-cities',
      body: request,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateDestinoDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoDto>({
      method: 'PUT',
      url: `/api/app/destino/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
