import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-star-rating',
  standalone: true,
  imports: [CommonModule],
  // CORRECCIÓN: Referencias a archivos sin ".component"
  templateUrl: './star-rating.html',
  styleUrls: ['./star-rating.scss']
})
export class StarRatingComponent {
  @Input() rating = 0;
  @Input() readonly = false;
  @Output() ratingChange = new EventEmitter<number>();

  stars = [1, 2, 3, 4, 5];
  hoverRating = 0;

  onMouseEnter(star: number) {
    if (!this.readonly) this.hoverRating = star;
  }

  onMouseLeave() {
    if (!this.readonly) this.hoverRating = 0;
  }

  rate(star: number) {
    if (!this.readonly) {
      this.rating = star;
      this.ratingChange.emit(this.rating);
    }
  }
}