import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CalificarModal } from './calificar-modal';

describe('CalificarModal', () => {
  let component: CalificarModal;
  let fixture: ComponentFixture<CalificarModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CalificarModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CalificarModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
