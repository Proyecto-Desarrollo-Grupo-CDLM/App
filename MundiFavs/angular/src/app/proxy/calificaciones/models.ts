import type { AuditedEntityDto } from '@abp/ng.core';

export interface CalificacionDto extends AuditedEntityDto<string> {
  estrellas: number;
  comentario?: string;
  destinoId?: string;
  userId?: string;
}

export interface CreateUpdateCalificacionDto {
  destinoId: string;
  puntuacion: number;
  comentario?: string;
}
