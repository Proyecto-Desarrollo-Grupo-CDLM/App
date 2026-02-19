import type { AuditedEntityDto, EntityDto } from '@abp/ng.core';

export interface ComentarioDto extends EntityDto<string> {
  estrellas: number;
  comentario?: string;
  autorNombre?: string;
  userId?: string;
  creationTime?: string;
}

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
  externalId: string;
}

export interface DestinoComentariosDto {
  destinoId?: string;
  nombreDestino?: string;
  puntuacionPromedio: number;
  totalCalificaciones: number;
  comentarios: ComentarioDto[];
}

export interface DestinoDto extends AuditedEntityDto<string> {
  nombre?: string;
  pais?: string;
  ciudad?: string;
  poblacion: number;
  ubicacion: CoordenadasDto;
  imageUrl: any;
  puntuacionPromedio: number;
}
