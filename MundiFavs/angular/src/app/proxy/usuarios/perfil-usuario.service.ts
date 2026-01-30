import type { UsuarioPublicoDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PerfilUsuarioService {
  apiName = 'Default';
  

  deleteMyAccount = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/perfil-usuario/my-account',
    },
    { apiName: this.apiName,...config });
  

  getPublicProfile = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UsuarioPublicoDto>({
      method: 'GET',
      url: `/api/app/perfil-usuario/${id}/public-profile`,
    },
    { apiName: this.apiName,...config });
  

  searchUsers = (filter: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, UsuarioPublicoDto[]>({
      method: 'POST',
      url: '/api/app/perfil-usuario/search-users',
      params: { filter },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
