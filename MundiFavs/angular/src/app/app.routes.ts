import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
  },
  {
    path: 'account',
    loadChildren: () => import('@abp/ng.account').then(c => c.createRoutes()),
  },
  {
    path: 'identity',
    loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
  },
  {
    path: 'setting-management',
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
  },
  
  // =======================================================
  // AGREGAMOS ESTA RUTA PARA CONECTAR EL MENÚ
  // =======================================================
  {
    path: 'city-search',  // Debe coincidir con el path del route.provider (sin la barra inicial)
    loadComponent: () => import('../../src/app/ciudades.component/ciudades.component') // La ubicación de tu archivo
      .then(c => c.CiudadesComponent), // El nombre de tu clase exportada
    canActivate: [authGuard, permissionGuard] // Opcional: Protege la ruta si no estás logueado
  },
];