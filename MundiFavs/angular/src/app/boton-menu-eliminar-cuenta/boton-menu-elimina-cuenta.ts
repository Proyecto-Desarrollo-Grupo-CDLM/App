import { Component } from '@angular/core';
import { Confirmation, ConfirmationService, ToasterService } from '@abp/ng.theme.shared';
import { AuthService } from '@abp/ng.core';
import { Router } from '@angular/router';
import { PerfilUsuarioService } from '../proxy/usuarios';

@Component({
  selector: 'app-delete-account-menu-item',
  standalone: true,
  template: `
    <a class="dropdown-item text-danger d-flex align-items-center pointer" (click)="onDelete()">
      <i class="fa fa-trash me-2"></i> 
      <span class="fw-bold">Eliminar Cuenta</span>
    </a>
  `,
  styles: [`
    .pointer { cursor: pointer; }
    /* Ajustes para que se vea bien en el menú */
    .dropdown-item {
        padding: 8px 16px;
        transition: background-color 0.2s;
    }
    .dropdown-item:hover {
        background-color: #fceceb; /* Un rojo muy suave al pasar el mouse */
    }
  `]
})
export class BotonMenuEliminaCuentaComponent {

  constructor(
    private confirmation: ConfirmationService,
    private toaster: ToasterService,
    private authService: AuthService,
    private router: Router,
    private PerfilUsuarioService: PerfilUsuarioService
  ) {}

  onDelete() {
    this.confirmation
      .warn('Esta acción es irreversible.', '¿Estás seguro de eliminar tu cuenta?', {
        yesText: 'Sí, eliminar',
        cancelText: 'Cancelar',
      })
      .subscribe((status) => {
        if (status === Confirmation.Status.confirm) {
          this.executeDelete();
        }
      });
  }

  private executeDelete() {
    // LLAMADA AL BACKEND
     this.PerfilUsuarioService.deleteMyAccount().subscribe(() => {
      
      this.toaster.success('Cuenta eliminada con éxito');
      this.authService.logout().subscribe(() => {
        this.router.navigate(['/']);
      });

     });
  }
}