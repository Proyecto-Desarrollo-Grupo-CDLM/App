import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToasterService } from '@abp/ng.theme.shared';

// Proxies
import { DestinoService, DestinoDto } from '../../proxy/destinos';
import { CalificacionService, CreateUpdateCalificacionDto } from '../../proxy/calificaciones';

// Tu Modal
import { CalificarModalComponent } from '../calificar-modal/calificar-modal';

@Component({
  selector: 'app-mis-destinos',
  standalone: true,
  imports: [CommonModule, CalificarModalComponent],
  templateUrl: './mis-destinos.html',
  styleUrls: ['./mis-destinos.scss']
})
export class MisDestinosComponent implements OnInit {
  misDestinos: DestinoDto[] = [];
  loading = false;
  
  // Variables Modal
  modalVisible = false;
  destinoSeleccionado: DestinoDto | null = null;
  
  // 👇 VARIABLES NUEVAS PARA EDICIÓN
  datosParaModal: { puntuacion: number, comentario: string } | null = null;
  idCalificacionExistente: string | null = null;

  constructor(
    private destinoService: DestinoService,
    private calificacionService: CalificacionService,
    private toaster: ToasterService
  ) {}

  ngOnInit() {
    this.cargarMisDestinos();
  }

  cargarMisDestinos() {
    this.loading = true;
    this.destinoService.getMyDestinations({ maxResultCount: 100 }).subscribe({
      next: (res) => {
        this.misDestinos = res.items || [];
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  // --- CALIFICAR ---
  abrirCalificar(destino: DestinoDto) {
    this.destinoSeleccionado = destino;

    // 1. Preguntamos al Backend si ya existe una reseña mía
    this.calificacionService.getMyCalificacion(destino.id).subscribe({
      next: (res) => {
        if (res) {
          // A) YA EXISTE: Preguntamos si quiere editar
          const confirmar = confirm(`Ya calificaste ${destino.nombre} con ${res.estrellas} estrellas. ¿Quieres modificar tu reseña?`);
          
          if (confirmar) {
            // Preparamos los datos para editar
            this.idCalificacionExistente = res.id;
            this.datosParaModal = { 
              puntuacion: res.estrellas, 
              comentario: res.comentario || '' 
            };
            this.modalVisible = true;
          }
        } else {
          // B) NO EXISTE: Abrimos modal vacío para crear nueva
          this.idCalificacionExistente = null;
          this.datosParaModal = null;
          this.modalVisible = true;
        }
      },
      error: (err) => {
        console.error(err);
        this.toaster.error('Error al verificar reseñas', 'Error');
      }
    });
  }

  guardarCalificacion(datos: { puntuacion: number, comentario: string }) {
    if (!this.destinoSeleccionado) return;

    const input: CreateUpdateCalificacionDto = {
      destinoId: this.destinoSeleccionado.id,
      puntuacion: datos.puntuacion,
      comentario: datos.comentario
    };

    if (this.idCalificacionExistente) {
      // --- CASO 1: EDITAR (UPDATE) ---
      this.calificacionService.update(this.idCalificacionExistente, input).subscribe({
        next: () => {
          this.toaster.info('¡Reseña actualizada!', 'Actualizado');
          this.cargarMisDestinos(); // Recargar para ver el promedio nuevo
          this.modalVisible = false;
        },
        error: () => this.toaster.error('No se pudo actualizar', 'Error')
      });
    } else {
      // --- CASO 2: CREAR (CREATE) ---
      this.calificacionService.create(input).subscribe({
        next: () => {
          this.toaster.success('¡Reseña publicada!', 'Éxito');
          this.cargarMisDestinos();
          this.modalVisible = false;
        },
        error: () => this.toaster.error('Error al guardar', 'Error')
      });
    }
  }
}