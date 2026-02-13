import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DestinoDto, DestinoService, CreateUpdateDestinoDto } from '../../proxy/destinos';
import { ToasterService, ConfirmationService, Confirmation } from '@abp/ng.theme.shared'; // <--- 1. Importar ConfirmationService
import { AuthService } from '@abp/ng.core';

@Component({
  selector: 'app-destinos-populares',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './destinos-populares.html',
  styleUrls: ['./destinos-populares.scss']
})
export class DestinosPopularesComponent implements OnInit {

  destinos: DestinoDto[] = [];
  loading = false;

  constructor(
    private destinoService: DestinoService,
    private toaster: ToasterService,
    private authService: AuthService,
    private confirmation: ConfirmationService // <--- 2. Inyectar Servicio de Confirmación
  ) { }

  ngOnInit(): void {
    this.cargarPopulares();
  }

  cargarPopulares(): void {
    this.loading = true;
    this.destinoService.getPopularDestinations(10).subscribe({
      next: (data) => {
        this.destinos = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error:', err);
        this.loading = false;
      }
    });
  }

  guardarEnFavoritos(destino: DestinoDto): void {
    // <--- 3. LÓGICA DEL CARTEL (MODAL) ---
    if (!this.authService.isAuthenticated) {
      this.confirmation
        .warn(
          'Para agregar este destino a tus favoritos, necesitas ingresar a tu cuenta.', // Mensaje
          '🔒 Iniciar Sesión' // Título
        )
        .subscribe((status) => {
          // Solo si el usuario hace clic en "Iniciar Sesión" (Confirmar)
          if (status === Confirmation.Status.confirm) {
            this.authService.navigateToLogin();
          }
        });
      return; // Detenemos aquí
    }
    // -------------------------------------

    console.log('Guardando popular:', destino.nombre);

    const nuevoFavorito: CreateUpdateDestinoDto = {
      nombre: destino.nombre,
      pais: destino.pais,
      ciudad: destino.ciudad,
      poblacion: destino.poblacion,
      imageUrl: destino.imageUrl,
      latitud: destino.ubicacion?.latitud || 0,
      longitud: destino.ubicacion?.longitud || 0
    };

    this.destinoService.create(nuevoFavorito).subscribe({
      next: () => {
        this.toaster.success(`¡${destino.nombre} guardada!`, 'Éxito');
      },
      error: (err) => {
        this.toaster.error('No se pudo guardar (quizás ya la tienes).', 'Info');
      }
    });
  }

  getRankClass(index: number): string {
    if (index === 0) return 'rank-gold';
    if (index === 1) return 'rank-silver';
    if (index === 2) return 'rank-bronze';
    return 'bg-dark';
  }
}