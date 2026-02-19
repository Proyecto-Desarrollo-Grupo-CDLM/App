import type { CreateFavoritoDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { DestinoDto } from '../destinos/models';

@Injectable({
  providedIn: 'root',
})
export class FavoritoService {
  apiName = 'Default';
  

  add = (input: CreateFavoritoDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/favorito',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinoDto[]>({
      method: 'GET',
      url: '/api/app/favorito',
    },
    { apiName: this.apiName,...config });
  

  remove = (destinoId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/favorito',
      params: { destinoId },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
