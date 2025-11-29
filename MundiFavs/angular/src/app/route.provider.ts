import { RoutesService, eLayoutType } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    // Se inyecta RoutesService dentro del AppInitializer
    const routesService = inject(RoutesService);
    configureRoutes(routesService)(); // Llama a la función que devuelve la función de configuración
  }),
];

// La función configureRoutes ahora recibe routesService como argumento,
// como en tu ejemplo más reciente.
function configureRoutes(routes: RoutesService) {
  return () => {
    routes.add([
      {
        path: '/',
        name: '::Menu:Home',
        iconClass: 'fas fa-home',
        order: 1,
        layout: eLayoutType.application,
      },
      // ==========================================================
      // NUEVA CATEGORÍA PADRE: Destinos
      // (Reemplaza a 'BookStore')
      // ==========================================================
      {
        path: '/Destinos',
        name: '::Menu:Destinos',
        iconClass: 'fas fa-globe-americas', // Ícono sugerido para Destinos
        order: 2,
        layout: eLayoutType.application,
        // Política de acceso sugerida (puedes ajustarla)
      requiredPolicy: 'MundiFavs.Destinos.Default', 
      },
      
      // ==========================================================
      // SUBMENÚ 1: Búsqueda de Ciudades (Operación 3.1)
      // (Reemplaza a '/books' y '/authors' de la plantilla)
      // ==========================================================
      {
        path: '/city-search',
        name: '::Menu:BuscarCiudades',
        parentName: '::Menu:Destinos', // Se cuelga del nuevo menú padre
        iconClass: 'fas fa-search',
        layout: eLayoutType.application,
        // Puedes dejar esta política si la búsqueda es pública, o usar una más específica
      requiredPolicy: 'MundiFavs.CitySearch', 
      },

      // ==========================================================
      // SUBMENÚ 2 (SUGERIDO): Gestión de Destinos Guardados (CRUD)
      // Aquí iría el componente DestinoComponent que adaptamos previamente
      // ==========================================================
      {
        path: '/mi-destinos',
        name: '::Menu:MiDestinos',
        parentName: '::Menu:Destinos',
        iconClass: 'fas fa-list',
        layout: eLayoutType.application,
        // Se asume que solo usuarios autenticados pueden ver su lista
        requiredPolicy: 'MundiFavs.Destinos.Manage', 
      }
    ]);
  };
}