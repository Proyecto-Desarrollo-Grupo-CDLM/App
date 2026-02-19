import type { NotificacionDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class NotificacionService {
  apiName = 'Default';
  

  crearNotificacionInterna = (usuarioId: string, tituloDestino: string, cambioDetectado: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/notificacion/crear-notificacion-interna/${usuarioId}`,
      params: { tituloDestino, cambioDetectado },
    },
    { apiName: this.apiName,...config });
  

  getMisNotificaciones = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, NotificacionDto[]>({
      method: 'GET',
      url: '/api/app/notificacion/mis-notificaciones',
    },
    { apiName: this.apiName,...config });
  

  marcarComoLeida = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/notificacion/${id}/marcar-como-leida`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
