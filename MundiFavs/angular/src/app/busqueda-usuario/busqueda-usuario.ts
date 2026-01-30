import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PerfilUsuarioService } from '../proxy/usuarios';
import { UsuarioPublicoDto} from '../proxy/usuarios/models'; 
import { Subject, debounceTime, distinctUntilChanged, switchMap } from 'rxjs';

@Component({
  selector: 'app-user-search',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './busqueda-usuario.html'
})
export class UserSearchComponent {
  searchText = '';
  users: UsuarioPublicoDto[] = [];
  private searchSubject = new Subject<string>();

  constructor(private perfilUsuarioService: PerfilUsuarioService, private router: Router) {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(filter => this.perfilUsuarioService.searchUsers(filter))
    ).subscribe(result => {
      this.users = result;
    });
  }

  onSearch(text: string) {
    this.searchSubject.next(text);
  }

  goToProfile(userId: string) {
    this.users = []; // Limpiamos resultados
    this.searchText = '';
    this.router.navigate(['/perfil-publico', userId]);
  }
}