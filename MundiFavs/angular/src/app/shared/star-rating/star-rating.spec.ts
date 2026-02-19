import { ComponentFixture, TestBed } from '@angular/core/testing';
// 👇 CORRECCIÓN: Importamos el nombre correcto "StarRatingComponent"
import { StarRatingComponent } from './star-rating';

describe('StarRatingComponent', () => {
  let component: StarRatingComponent;
  let fixture: ComponentFixture<StarRatingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      // Como es 'standalone: true', lo importamos, no lo declaramos
      imports: [StarRatingComponent] 
    })
    .compileComponents();

    fixture = TestBed.createComponent(StarRatingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});