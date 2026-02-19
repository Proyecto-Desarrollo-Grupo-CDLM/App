import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; // 👈 VITAL para que funcione *ngIf y *ngFor
import { RestService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-notificaciones',
  standalone: true, // 👈 Alineado a tu app.component
  imports: [CommonModule], // 👈 Inyectamos las directivas básicas de Angular
  templateUrl: './notificaciones.html',
  styleUrls: ['./notificaciones.scss'] // O .css dependiendo de cómo lo generaste
})
export class NotificacionesComponent implements OnInit {
  notificaciones: any[] = [];
  cargando = true;

  constructor(
    private restService: RestService,
    private toaster: ToasterService
  ) {}

  ngOnInit(): void {
    this.cargarNotificaciones();
  }

  cargarNotificaciones() {
    this.cargando = true;
    this.restService.request<any, any[]>({
      method: 'GET',
      url: '/api/app/notificacion/mis-notificaciones',
    }).subscribe({
      next: (data) => {
        // Guardamos los datos recibidos del backend en nuestra variable
        this.notificaciones = data;
        this.cargando = false;
        console.log('Notificaciones cargadas en pantalla:', data); // 👈 Para depurar
      },
      error: (err) => {
        console.error('Error cargando la lista:', err);
        this.cargando = false;
      }
    });
  }

  marcarComoLeida(id: string) {
    this.restService.request<any, void>({
      method: 'POST', 
      url: `/api/app/notificacion/${id}/marcar-como-leida`,
    }).subscribe({
      next: () => {
        this.toaster.success('¡Marcada como leída!');
        this.cargarNotificaciones(); // Recargamos la lista para actualizar la vista
      },
      error: (err) => {
        this.toaster.error('Hubo un problema al actualizar la notificación.');
      }
    });
  }
}