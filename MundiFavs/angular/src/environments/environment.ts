import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44350/',
  redirectUri: baseUrl,
  clientId: 'MundiFavs_App',
  responseType: 'code',
  scope: 'offline_access MundiFavs',
  requireHttps: true,
};

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'MundiFavs',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44350',
      rootNamespace: 'MundiFavs',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
} as Environment;
