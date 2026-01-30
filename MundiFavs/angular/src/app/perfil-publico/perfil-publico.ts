import { Component, OnInit } from '@angular/core';
import { CommonModule, Location } from '@angular/common'; // Importamos Location
import { ActivatedRoute, RouterModule } from '@angular/router';
import { PerfilUsuarioService } from '../proxy/usuarios';

@Component({
  selector: 'app-perfil-publico',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './perfil-publico.html',
  styleUrls: ['./perfil-publico.scss']
})
export class PerfilPublicoComponent implements OnInit {
  user: any = null;

  constructor(
    private route: ActivatedRoute,
    private userProfileService: PerfilUsuarioService,
    private location: Location // Inyectamos el servicio de ubicación
  ) {}

  ngOnInit(): void {
    // Escuchamos los parámetros de la URL de forma reactiva
    this.route.params.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.userProfileService.getPublicProfile(id).subscribe({
          next: (result) => {
            this.user = result;
          },
          error: (err) => {
            console.error("No se pudo cargar el perfil público", err);
          }
        });
      }
    });
  }

  // Método para el botón de regresar
  regresar(): void {
    this.location.back();
  }
}