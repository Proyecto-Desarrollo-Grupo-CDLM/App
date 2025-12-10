import type { AuditedEntityDto } from '@abp/ng.core';

export interface CoordenadasDto {
  latitud: number;
  longitud: number;
}

export interface CreateUpdateDestinoDto {
  nombre: string;
  pais: string;
  ciudad: string;
  poblacion: number;
  latitud: number;
  longitud: number;
  imageUrl: string;
}

export interface DestinoDto extends AuditedEntityDto<string> {
  nombre?: string;
  pais?: string;
  ciudad?: string;
  poblacion: number;
  ubicacion: CoordenadasDto;
  imageUrl: any;
}
