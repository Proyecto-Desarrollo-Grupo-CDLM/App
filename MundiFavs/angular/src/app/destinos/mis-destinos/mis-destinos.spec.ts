import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs'; // Necesario para simular respuestas

// 1. Importamos el nombre CORRECTO del componente
import { MisDestinosComponent } from './mis-destinos';

// 2. Importamos los servicios que usa tu componente para "falsificarlos" en la prueba
import { DestinoService } from '../../proxy/destinos';
import { CalificacionService } from '../../proxy/calificaciones';
import { ToasterService } from '@abp/ng.theme.shared';

describe('MisDestinosComponent', () => {
  let component: MisDestinosComponent;
  let fixture: ComponentFixture<MisDestinosComponent>;

  // Creamos servicios falsos (Mocks) para que el test no falle pidiendo backend
  const mockDestinoService = {
    getMyDestinations: () => of({ items: [], totalCount: 0 })
  };
  
  const mockCalificacionService = {
    create: () => of({})
  };

  const mockToasterService = {
    success: () => {},
    error: () => {}
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      // Importamos el componente Standalone
      imports: [MisDestinosComponent],
      // Proveemos los servicios falsos
      providers: [
        { provide: DestinoService, useValue: mockDestinoService },
        { provide: CalificacionService, useValue: mockCalificacionService },
        { provide: ToasterService, useValue: mockToasterService }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MisDestinosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});