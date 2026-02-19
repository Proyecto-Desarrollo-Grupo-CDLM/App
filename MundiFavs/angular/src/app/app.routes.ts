import { authGuard, permissionGuard } from '@abp/ng.core';
import { Routes } from '@angular/router';

// Tus componentes existentes (Mantenemos tus rutas de importación exactas)
import { CiudadesComponent } from './ciudades.component/ciudades.component'; 
import { DestinosPopularesComponent } from './destinos/destinos-populares/destinos-populares';
import { MisDestinosComponent } from './destinos/mis-destinos/mis-destinos'; 
import { PerfilPublicoComponent } from './perfil-publico/perfil-publico';
import { UserSearchComponent } from './busqueda-usuario/busqueda-usuario';
import { DeleteAccountComponent } from './eliminar-cuenta/eliminar-cuenta';

export const APP_ROUTES: Routes = [
  {
    path: '',
    pathMatch: 'full',
    loadComponent: () => import('./home/home.component').then(c => c.HomeComponent),
  },
  
  // Ruta existente (La dejamos tal cual)
  {
    path: 'city-search',
    component: CiudadesComponent
  },

  // --- AGREGADO: Ruta para que funcione el botón del Home ---
  {
    path: 'ciudades',
    component: CiudadesComponent
  },
  // ---------------------------------------------------------

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
  {
    path: 'mis-destinos',
    component: MisDestinosComponent,
    title: 'Mis Destinos Guardados'
  },
  {
    path: 'buscar-usuarios',
    component: UserSearchComponent
  },
  {
    path: 'eliminar-cuenta',
    component: DeleteAccountComponent
  },
  {
    path: 'perfil-publico/:id', 
    loadComponent: () =>
      import('./perfil-publico/perfil-publico').then(m => m.PerfilPublicoComponent),
  },
 {
  path: 'notificaciones',
  loadComponent: () => import('./notificaciones/notificaciones').then(m => m.NotificacionesComponent),
  // Si usas modulos (NgModule), sería loadChildren. Si es standalone, es loadComponent.
}
];