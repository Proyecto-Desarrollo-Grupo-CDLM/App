import { RoutesService, eLayoutType } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    // Se inyecta RoutesService dentro del AppInitializer
    const routesService = inject(RoutesService);
    configureRoutes(routesService)(); // Llama a la función que devuelve la función de configuración
  }),
];


function configureRoutes(routes: RoutesService) 
{
  return () => {
    routes.add([
      {
        path: '/',
        name: '::Menu:Home',
        iconClass: 'fas fa-home',
        order: 1,
        layout: eLayoutType.application,
      },
      
      {
        path: '/city-search',       
        name: 'Buscar Ciudades',    
        iconClass: 'fas fa-search', 
        order: 2,                   
        layout: eLayoutType.application,
      },
    
      {
        path: '/destinos-populares',
        name: 'Destinos Populares', // El texto que se ve en el menú
        iconClass: 'fas fa-fire',   // Icono de fueguito
        order: 2,
        layout: eLayoutType.application,
      },
    
      {
        path: '/buscar-usuarios',
        name: 'Buscar Usuarios',
        iconClass: 'fas fa-user',
        order: 2,
        layout: eLayoutType.application,
      },

      // --- BOTÓN 2: MIS DESTINOS ---
      {
        path: '/mis-destinos',
        name: 'Mis Destinos',       // El texto que se ve en el menú
        iconClass: 'fas fa-suitcase-rolling', // Icono de maleta
        order: 3,
        layout: eLayoutType.application,
      },  
    ]);
  };
}