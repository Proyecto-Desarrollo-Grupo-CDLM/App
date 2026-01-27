import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StarRatingComponent } from '../../shared/star-rating/star-rating'; 

@Component({
  selector: 'app-calificar-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, StarRatingComponent],
  templateUrl: './calificar-modal.html',
  styleUrls: ['./calificar-modal.scss']
})
export class CalificarModalComponent implements OnChanges {
  @Input() visible = false;
  @Input() destinoNombre = '';
  
  // 👇 NUEVO: Recibimos datos si es una edición (si es null, es nuevo)
  @Input() datosEdicion: { puntuacion: number, comentario: string } | null = null;
  
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() onSave = new EventEmitter<{puntuacion: number, comentario: string}>();

  puntuacion = 0;
  comentario = '';

  // 👇 DETECTAMOS CUANDO SE ABRE EL MODAL
  ngOnChanges(changes: SimpleChanges) {
    if (changes['visible'] && this.visible) {
      if (this.datosEdicion) {
        // MODO EDICIÓN: Cargamos los datos que ya existían
        this.puntuacion = this.datosEdicion.puntuacion;
        this.comentario = this.datosEdicion.comentario || '';
      } else {
        // MODO CREAR: Limpiamos todo
        this.puntuacion = 0;
        this.comentario = '';
      }
    }
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(false);
  }

  save() {
    if (this.puntuacion > 0) {
      this.onSave.emit({
        puntuacion: this.puntuacion,
        comentario: this.comentario
      });
      this.close();
    }
  }
}