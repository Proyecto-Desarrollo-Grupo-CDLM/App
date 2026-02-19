import type { EventoDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class EventoService {
  apiName = 'Default';
  

  buscarEnTicketmaster = (ciudad: string, keyword?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EventoDto[]>({
      method: 'GET',
      url: '/api/app/evento/ticketmaster',
      params: { ciudad, keyword },
    },
    { apiName: this.apiName,...config });
  

  guardarEvento = (input: EventoDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EventoDto>({
      method: 'POST',
      url: '/api/app/evento/guardar-evento',
      body: input,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
