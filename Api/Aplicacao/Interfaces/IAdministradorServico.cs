namespace Api;

public interface IAdministradorServico
{
    Administrador Login(LoginDTO loginDTO);
    Administrador Incluir(Administrador administrador);
    Administrador BuscarPorId(int Id);
    List<Administrador> Todos(int? pagina);

}
