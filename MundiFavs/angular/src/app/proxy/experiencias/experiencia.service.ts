import type { GetExperienciasInput } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ExperienciaService {
  apiName = 'Default';
  

  create = (input: CreateUpdateExperienciaDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienciaDto>({
      method: 'POST',
      url: '/api/app/experiencia',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/experiencia/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetExperienciasInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ExperienciaDto>>({
      method: 'GET',
      url: '/api/app/experiencia',
      params: { destinoId: input.destinoId, filtroTexto: input.filtroTexto, valoracion: input.valoracion, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateExperienciaDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienciaDto>({
      method: 'PUT',
      url: `/api/app/experiencia/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
