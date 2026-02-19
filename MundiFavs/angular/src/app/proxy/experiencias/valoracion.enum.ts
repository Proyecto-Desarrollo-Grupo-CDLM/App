import { mapEnumToOptions } from '@abp/ng.core';

export enum Valoracion {
  Neutral = 0,
  Positiva = 1,
  Negativa = 2,
}

export const valoracionOptions = mapEnumToOptions(Valoracion);
