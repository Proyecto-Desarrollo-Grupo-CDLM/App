import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '@abp/ng.core';
import { ToasterService, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';

// Proxies
import { DestinoDto, DestinoService, CreateUpdateDestinoDto } from '../../proxy/destinos';
import { CalificacionService, CreateUpdateCalificacionDto } from '../../proxy/calificaciones'; // <--- NUEVO IMPORT

// Componentes (Sin .component en la ruta)
import { CalificarModalComponent } from '../calificar-modal/calificar-modal'; // <--- NUEVO IMPORT

@Component({
  selector: 'app-destinos-populares',
  standalone: true,
  imports: [CommonModule, CalificarModalComponent], // <--- AGREGADO AQUÍ
  templateUrl: './destinos-populares.html',
  styleUrls: ['./destinos-populares.scss']
})
export class DestinosPopularesComponent implements OnInit {

  destinos: DestinoDto[] = [];
  loading = false;

  // --- VARIABLES PARA EL MODAL ---
  modalVisible = false;
  destinoSeleccionado: DestinoDto | null = null;

  constructor(
    private destinoService: DestinoService,
    private calificacionService: CalificacionService, // <--- INYECCIÓN DEL SERVICIO
    private toaster: ToasterService,
    private authService: AuthService,
    private confirmation: ConfirmationService
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

  // --- FAVORITOS (Tu lógica original) ---
  guardarEnFavoritos(destino: DestinoDto): void {
    if (!this.checkLogin()) return;

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

  // --- CALIFICACIONES (Nueva Lógica) ---

  abrirCalificar(destino: DestinoDto) {
    if (!this.checkLogin()) return;

    this.destinoSeleccionado = destino;
    this.modalVisible = true;
  }

  guardarCalificacion(datos: { puntuacion: number, comentario: string }) {
    if (!this.destinoSeleccionado) return;

    const input: CreateUpdateCalificacionDto = {
      destinoId: this.destinoSeleccionado.id,
      puntuacion: datos.puntuacion,
      comentario: datos.comentario
    };

    this.calificacionService.create(input).subscribe({
      next: () => {
        this.toaster.success('¡Gracias por tu opinión!', 'Reseña enviada');
        this.modalVisible = false; // Cerramos el modal
      },
      error: (err) => {
        console.error(err);
        this.toaster.error('Ocurrió un error al guardar tu reseña.', 'Error');
      }
    });
  }

  // Helper para reutilizar la validación de sesión
  private checkLogin(): boolean {
    if (!this.authService.isAuthenticated) {
      this.confirmation
        .warn(
          'Necesitas ingresar a tu cuenta para realizar esta acción.',
          '🔒 Iniciar Sesión'
        )
        .subscribe((status) => {
          if (status === Confirmation.Status.confirm) {
            this.authService.navigateToLogin();
          }
        });
      return false;
    }
    return true;
  }

  getRankClass(index: number): string {
    if (index === 0) return 'rank-gold';
    if (index === 1) return 'rank-silver';
    if (index === 2) return 'rank-bronze';
    return 'bg-dark';
  }
}