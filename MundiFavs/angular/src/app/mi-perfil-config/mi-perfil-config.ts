import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { AccountModule } from '@abp/ng.account'; // <--- IMPORTANTE: Importar esto
import { eAccountComponents } from '@abp/ng.account';
import { DeleteAccountComponent } from '../eliminar-cuenta/eliminar-cuenta';

@Component({
  selector: 'app-mi-perfil-config',
  standalone: true,
  imports: [
    CommonModule,
    CoreModule,
    ThemeSharedModule,
    AccountModule, // <--- Agrégalo aquí para que funcionen los formularios
    DeleteAccountComponent 
  ],
  templateUrl: './mi-perfil-config.html',
  styleUrls: ['./mi-perfil-config.scss'] // Asegúrate de crear este archivo si no existe
})
export class MiPerfilConfigComponent {
  personalSettingsKey = eAccountComponents.PersonalSettings;
  changePasswordKey = eAccountComponents.ChangePassword;
  
  selectedTab = 0; 
}