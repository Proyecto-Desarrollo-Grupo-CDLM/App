import type { EntityDto } from '@abp/ng.core';

export interface NotificacionDto extends EntityDto<string> {
  tituloDestino?: string;
  cambioDetectado?: string;
  leida: boolean;
  fecha?: string;
  hora?: string;
}
