import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserMenuService, ToasterService } from '@abp/ng.theme.shared';
import { CoreModule, RestService } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { Subscription, timer } from 'rxjs'; // 👈 Cambiamos a 'timer' que es más limpio
import { switchMap } from 'rxjs/operators';

// IMPORTA TU NUEVO COMPONENTE
import { BotonMenuEliminaCuentaComponent } from './boton-menu-eliminar-cuenta/boton-menu-elimina-cuenta';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule],
  template: `
    <abp-loader-bar></abp-loader-bar>
    <abp-dynamic-layout></abp-dynamic-layout>
  `,
})
export class AppComponent implements OnInit, OnDestroy {
  // Variable para guardar la suscripción
  private pollingSubscription: Subscription;

  // 👇 NUEVO: Usamos un Set (conjunto) para guardar los IDs de las notificaciones que ya mostramos.
  // Así evitamos repetir el mismo Toast en pantalla si el usuario no lo marca como leído.
  private notificacionesMostradas = new Set<string>();

  constructor(
    private userMenu: UserMenuService,
    private toaster: ToasterService, 
    private restService: RestService 
  ) {}

  ngOnInit() {
    this.configureUserMenu();
    this.iniciarSondeoNotificaciones(); // 👈 Arrancamos el motor de notificaciones
  }

  ngOnDestroy() {
    if (this.pollingSubscription) {
      this.pollingSubscription.unsubscribe();
    }
  }

  private configureUserMenu() {
    this.userMenu.addItems([
      {
        id: 'DeleteAccount',
        order: 10000,
        component: BotonMenuEliminaCuentaComponent,
      },
    ]);
  }

  // 👇 LÓGICA DE SONDEO (POLLING) MEJORADA
  private iniciarSondeoNotificaciones() {
    // timer(0, 60000) = Ejecutar inmediatamente al cargar (0) y luego cada 60 segundos (60000)
    this.pollingSubscription = timer(0, 60000)
      .pipe(
        switchMap(() => {
          return this.restService.request<any, any[]>({
            method: 'GET',
            url: '/api/app/notificacion/mis-notificaciones', 
          });
        })
      )
      .subscribe({
        next: (listaNotificaciones) => {
          // 1. Filtramos solo las que no están leídas
          const noLeidas = listaNotificaciones.filter(n => !n.leida);

          // 2. Recorremos las no leídas
          noLeidas.forEach((notif) => {
            // 3. Si el ID de esta notificación NO está en nuestro registro de "ya mostradas"
            if (!this.notificacionesMostradas.has(notif.id)) {
              
              // Mostramos la alerta en pantalla
              this.toaster.info(
                notif.cambioDetectado, 
                `¡Alerta en ${notif.tituloDestino}!`,
                { life: 8000 } // Dura 8 segundos en pantalla
              );
              
              // Agregamos el ID al Set para no volver a mostrar el Toast el próximo minuto
              this.notificacionesMostradas.add(notif.id);
            }
          });
        },
        error: (err) => {
          // Silenciamos el error si el usuario no está logueado
          console.debug('Polling silenciado: Usuario no autenticado.');
        }
      });
  }
}