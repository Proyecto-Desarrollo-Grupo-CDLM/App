import { Component, OnInit } from '@angular/core';
import { UserMenuService } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';

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
export class AppComponent implements OnInit {

  constructor(private userMenu: UserMenuService) {}

  ngOnInit() {
    this.configureUserMenu();
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
}