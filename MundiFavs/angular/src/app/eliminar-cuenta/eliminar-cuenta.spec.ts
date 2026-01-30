import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EliminarCuenta } from './eliminar-cuenta';

describe('EliminarCuenta', () => {
  let component: EliminarCuenta;
  let fixture: ComponentFixture<EliminarCuenta>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EliminarCuenta]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EliminarCuenta);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
