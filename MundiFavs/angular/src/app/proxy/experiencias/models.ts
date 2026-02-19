import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { Valoracion } from './valoracion.enum';

export interface GetExperienciasInput extends PagedAndSortedResultRequestDto {
  destinoId?: string;
  filtroTexto?: string;
  valoracion?: Valoracion;
}
