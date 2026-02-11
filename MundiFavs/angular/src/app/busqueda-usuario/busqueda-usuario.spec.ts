import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BusquedaUsuario } from './busqueda-usuario';

describe('BusquedaUsuario', () => {
  let component: BusquedaUsuario;
  let fixture: ComponentFixture<BusquedaUsuario>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BusquedaUsuario]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BusquedaUsuario);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
