import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

// Tus componentes existentes
import { CiudadesComponent } from './ciudades.component/ciudades.component'; 

// 1. IMPORTA EL NUEVO COMPONENTE DE DESTINOS POPULARES
import { DestinosPopularesComponent } from './destinos/destinos-populares/destinos-populares'

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
  },
  
  {
    path: 'city-search',
    component: CiudadesComponent
  },

  // 2. AGREGA ESTA RUTA NUEVA:
  {
    path: 'destinos-populares',
    component: DestinosPopularesComponent
  },

  // Rutas de ABP (Account, Identity, etc.)
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
];