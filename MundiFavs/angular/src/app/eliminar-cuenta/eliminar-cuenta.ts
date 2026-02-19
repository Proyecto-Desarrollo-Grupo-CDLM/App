import { Component } from '@angular/core';
import { PerfilUsuarioService } from '../proxy/usuarios'; // Ajusta la ruta a tu proxy
import { ConfirmationService, ToasterService, Confirmation} from '@abp/ng.theme.shared';
import { AuthService } from '@abp/ng.core';

@Component({
  selector: 'app-delete-account',
  standalone: true,
  template: `
    <div class="p-3">
      <h5>Eliminar mi cuenta</h5>
      <p class="text-danger">Esta acción marcará su cuenta como inactiva. No podrá volver a ingresar.</p>
      <button class="btn btn-danger" (click)="delete()">Confirmar Eliminación</button>
    </div>
  `
})
export class DeleteAccountComponent {
  constructor(
    private accountService: PerfilUsuarioService, 
    private toaster: ToasterService,
    private confirmation: ConfirmationService,
    private authService: AuthService
  ) {}

  delete() {
    this.confirmation.warn(
        '¿Está seguro de que desea eliminar su cuenta?',
        'Confirmar eliminación'
    ).subscribe((status: Confirmation.Status) => { // Tipamos el status
      
      // Comparamos usando el Enum de ABP en lugar de un número mágico
      if (status === Confirmation.Status.confirm) { 
        this.accountService.deleteMyAccount().subscribe({
          next: () => {
            this.toaster.success('Cuenta eliminada con éxito.');
            this.authService.logout().subscribe();
          },
          error: () => {
            this.toaster.error('No se pudo procesar la solicitud.');
          }
        });
      }
    });
  }
}