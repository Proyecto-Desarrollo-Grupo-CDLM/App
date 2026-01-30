import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

// Tus componentes existentes
import { CiudadesComponent } from './ciudades.component/ciudades.component'; 

// 1. IMPORTA EL NUEVO COMPONENTE DE DESTINOS POPULARES
import { DestinosPopularesComponent } from './destinos/destinos-populares/destinos-populares'

import { PerfilPublicoComponent } from './perfil-publico/perfil-publico';
import { UserSearchComponent } from './busqueda-usuario/busqueda-usuario';
import {DeleteAccountComponent} from './eliminar-cuenta/eliminar-cuenta';
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

 
  {
    path: 'destinos-populares',
    component: DestinosPopularesComponent
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

  
{path: 'buscar-usuarios',
  component: UserSearchComponent},

  {path: 'eliminar-cuenta',
  component: DeleteAccountComponent},
  {
    path: 'perfil-publico/:id', 
    loadComponent: () =>
      import('./perfil-publico/perfil-publico').then(m => m.PerfilPublicoComponent),
  },
  /*{
  path: 'mis-ajustes',
  loadComponent: () => import('./mis-ajustes/mis-ajustes').then(m => m.MisAjustesComponent),
  canActivate: [authGuard] // Solo para usuarios logueados
},*/

];