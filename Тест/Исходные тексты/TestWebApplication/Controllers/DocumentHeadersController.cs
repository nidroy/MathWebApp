using Microsoft.AspNetCore.Mvc;
using System;
using TestWebApplication.Models;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Route("documentHeaders")]
    public class DocumentHeadersController : Controller
    {
        // Поле для хранения контекста базы данных
        private ApplicationContext _context;

        // Конструктор контроллера, принимающий контекст базы данных в качестве параметра
        public DocumentHeadersController(ApplicationContext context)
        {
            _context = context;
        }

        // Метод для отображения списка всех заголовков документов
        public IActionResult Index()
        {
            try
            {
                // Получаем список всех заголовков документов из базы данных
                List<document_header> document_headers = _context.document_header.ToList();

                // Для каждого заголовка документа находим соответствующий справочник контрагента
                foreach (var document_header in document_headers)
                {
                    counterparty counterparty = _context.counterparty.FirstOrDefault(c => c.id == document_header.counterparty_id);
                    document_header.counterparty = counterparty;
                }

                // Возвращаем представление с переданным списком заголовков документов
                return View("Index", document_headers);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для чтения информации о конкретном заголовке документа по его id
        [HttpGet("read/{id}")]
        public IActionResult Read(int id)
        {
            try
            {
                // Находим заголовок документа по id
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == id);
                // Находим справочник контрагента соответствующий заголовоку документа
                counterparty counterparty = _context.counterparty.FirstOrDefault(c => c.id == document_header.counterparty_id);
                document_header.counterparty = counterparty;

                // Возвращаем представление с информацией о заголовке документа
                return View("Read", document_header);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для отображения формы создания нового заголовка документа
        [HttpGet("create")]
        public IActionResult Create()
        {
            try
            {
                // Получаем список всех справочников контрагентов из базы данных
                List<counterparty> counterparties = _context.counterparty.ToList();
                // Создаем временную модель с дополнительными полями "типы документа", "состояния доккумента", "справочники контрагентов"
                document_header_view_model document_header = new()
                {
                    document_header = new(),
                    counterparties = counterparties,
                    document_types = new List<string>()
                    {
                        "Приход",
                        "Резерв",
                        "Расход"
                    },
                    document_statuses = new List<string>()
                    {
                        "Черновик",
                        "Оприходовано",
                        "Зарезервировано",
                        "Списано"
                    }
                };
                // Возвращаем представление формы создания заголовка документа
                return View("Create", document_header);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для создания нового заголовка документа
        [HttpPost("create")]
        public IActionResult Create(document_header document_header)
        {
            try
            {
                // Проверяем корректность данных заголовка документа
                if (document_header == null)
                {
                    return BadRequest("Incorrect document header data provided");
                }

                // Устанавливаем дату документа в формате UTC
                document_header.document_date = DateTime.SpecifyKind(document_header.document_date, DateTimeKind.Utc);
                _context.document_header.Add(document_header);
                _context.SaveChanges();

                // Перенаправляем на метод Index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для отображения формы редактирования заголовка документа по id
        [HttpGet("update/{id}")]
        public IActionResult Update(int id)
        {
            try
            {
                // Находим заголовок документа по id
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == id);
                counterparty counterparty = _context.counterparty.FirstOrDefault(c => c.id == document_header.counterparty_id);
                document_header.counterparty = counterparty;

                // Получаем список всех контрагентов из базы данных
                List<counterparty> counterparties = _context.counterparty.ToList();
                // Создаем временную модель с дополнительными полями "типы документа", "состояния доккумента"
                document_header_view_model document_header_view_model = new()
                {
                    document_header = document_header,
                    counterparties = counterparties,
                    document_types = new List<string>()
                    {
                        "Приход",
                        "Резерв",
                        "Расход"
                    },
                    document_statuses = new List<string>()
                    {
                        "Черновик",
                        "Оприходовано",
                        "Зарезервировано",
                        "Списано"
                    }
                };

                // Возвращаем представление формы редактирования заголовка документа
                return View("Update", document_header_view_model);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для обновления заголовка документа
        [HttpPost("update/{id}")]
        public IActionResult Update(document_header_view_model document_header_data)
        {
            try
            {
                // Проверяем корректность данных заголовка документа
                if (document_header_data.document_header == null)
                {
                    return BadRequest("Incorrect document header data provided");
                }

                // Находим заголовок документа по id
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == document_header_data.document_header.id);

                // Обновляем данные заголовка документа
                document_header.document_number = document_header_data.document_header.document_number;
                document_header.counterparty_id = document_header_data.document_header.counterparty_id;
                document_header.document_date = DateTime.SpecifyKind(document_header_data.document_header.document_date, DateTimeKind.Utc);
                document_header.document_type = document_header_data.document_header.document_type;
                document_header.document_status = document_header_data.document_header.document_status;

                _context.document_header.Update(document_header);
                _context.SaveChanges();

                // Перенаправляем на метод Index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для удаления заголовка документа по id
        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                // Находим заголовок документа по id
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == id);

                // Получаем список всех строк документа
                List<document_line> document_lines = _context.document_line.ToList();

                // Удаляем строки документа, связанные с заголовком
                foreach (var document_line in document_lines)
                {
                    if (document_line.document_header_id == id)
                    {
                        _context.document_line.Remove(document_line);
                        _context.SaveChanges();
                    }
                }

                // Удаляем заголовок документа
                _context.document_header.Remove(document_header);
                _context.SaveChanges();

                // Перенаправляем на метод Index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для приходования документа по id
        [HttpGet("receiveDocument/{id}")]
        public IActionResult ReceiveDocument(int id)
        {
            try
            {
                // Находим заголовок документа по id
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == id);

                // Получаем список всех строк документа
                List<document_line> document_lines = _context.document_line.ToList();

                // Для каждой строчки в документе на величину поля «количество» увеличивается величина поля «количество» в таблице складских остатков
                foreach (var document_line in document_lines)
                {
                    if (document_line.document_header_id == id)
                    {
                        product_stock product_stock = _context.product_stock.FirstOrDefault(ps => ps.product_id == document_line.product_id);

                        product_stock.actual_quantity += document_line.quantity;

                        _context.product_stock.Update(product_stock);
                        _context.SaveChanges();
                    }
                }

                // Обновляем состояние документа на "Оприходовано"
                document_header.document_status = "Оприходовано";

                _context.document_header.Update(document_header);
                _context.SaveChanges();

                // Перенаправляем на метод Index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для отмены приходования документа по id
        [HttpGet("cancelReceivingDocument/{id}")]
        public IActionResult CancelReceivingDocument(int id)
        {
            try
            {
                // Находим заголовок документа по id
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == id);

                // Получаем список всех строк документа
                List<document_line> document_lines = _context.document_line.ToList();

                // Для каждой строчки в документе на величину поля «количество» уменьшается величина поля «количество» в таблице складских остатков
                foreach (var document_line in document_lines)
                {
                    if (document_line.document_header_id == id)
                    {
                        product_stock product_stock = _context.product_stock.FirstOrDefault(ps => ps.product_id == document_line.product_id);

                        product_stock.actual_quantity -= document_line.quantity;

                        _context.product_stock.Update(product_stock);
                        _context.SaveChanges();
                    }
                }

                // Обновляем состояние документа на "Черновик"
                document_header.document_status = "Черновик";

                _context.document_header.Update(document_header);
                _context.SaveChanges();

                // Перенаправляем на метод Index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для резервирования документа по id
        [HttpGet("reserveDocument/{id}")]
        public IActionResult ReserveDocument(int id)
        {
            try
            {
                // Находим заголовок документа по id
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == id);

                // Получаем список всех строк документа
                List<document_line> document_lines = _context.document_line.ToList();

                // Реализуем алгоритм для каждой строчки в документе
                foreach (var document_line in document_lines)
                {
                    if (document_line.document_header_id == id)
                    {
                        // Разница между значениями полей «количество» и «количество в резерве»
                        int quantity = document_line.quantity - document_line.reserved_quantity;

                        product_stock product_stock = _context.product_stock.FirstOrDefault(ps => ps.product_id == document_line.product_id);

                        // На величину разницы увеличиваем значение поля «количество в резерве» в таблице складских остатков
                        product_stock.reserved_quantity += quantity;

                        // Не превышаем значение поля «количество фактическое»
                        if (product_stock.reserved_quantity > product_stock.actual_quantity)
                        {
                            quantity -= product_stock.reserved_quantity - product_stock.actual_quantity;
                            product_stock.reserved_quantity = product_stock.actual_quantity;
                        }

                        _context.product_stock.Update(product_stock);
                        _context.SaveChanges();

                        // Вычисляем сумму строки документа
                        decimal amount = DocumentLinesController.CalculateDocumentAmount(document_line);

                        // На величину количества, которое удалось зарезервировать, увеличиваем значение поля «количество в резерве» для строчки документа
                        document_line.reserved_quantity += quantity;

                        _context.document_line.Update(document_line);
                        _context.SaveChanges();

                        // Перерасчет суммы документа
                        DocumentLinesController.UpdateDocumentAmount(_context, document_header, document_line, amount);
                    }
                }

                // Обновляем состояние документа на "Зарезервировано"
                document_header.document_status = "Зарезервировано";

                _context.document_header.Update(document_header);
                _context.SaveChanges();

                // Перенаправляем на метод Index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для отмены резервирования документа по id
        [HttpGet("cancelReservingDocument/{id}")]
        public IActionResult CancelReservingDocument(int id)
        {
            try
            {
                // Находим заголовок документа по id
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == id);

                // Получаем список всех строк документа
                List<document_line> document_lines = _context.document_line.ToList();

                // Реализуем алгоритм для каждой строчки в документе
                foreach (var document_line in document_lines)
                {
                    if (document_line.document_header_id == id)
                    {
                        product_stock product_stock = _context.product_stock.FirstOrDefault(ps => ps.product_id == document_line.product_id);

                        // На величину поля «количество в резерве» уменьшаем для соответствующего товара значение поля «количество в резерве» в таблице складских остатков
                        product_stock.reserved_quantity -= document_line.reserved_quantity;

                        _context.product_stock.Update(product_stock);
                        _context.SaveChanges();

                        // Вычисляем сумму строки документа
                        decimal amount = DocumentLinesController.CalculateDocumentAmount(document_line);

                        // Уменьшаем до нуля значение поля «количество в резерве» для строчки документа 
                        document_line.reserved_quantity = 0;

                        _context.document_line.Update(document_line);
                        _context.SaveChanges();

                        // Перерасчет суммы документа
                        DocumentLinesController.UpdateDocumentAmount(_context, document_header, document_line, amount);
                    }
                }

                // Обновляем состояние документа на "Черновик"
                document_header.document_status = "Черновик";

                _context.document_header.Update(document_header);
                _context.SaveChanges();

                // Перенаправляем на метод Index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для списания документа по id
        [HttpGet("writeOffDocument/{id}")]
        public IActionResult WriteOffDocument(int id)
        {
            try
            {
                // Находим заголовок документа по id
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == id);

                // Получаем список всех строк документа
                List<document_line> document_lines = _context.document_line.ToList();

                // Список измененных складских остатков
                List<product_stock> product_stocks = new();

                // Для каждой строчки в документе на величину поля «количество» уменьшаем величина полей «количество» и «количество в резерве» в таблице складских остатков
                foreach (var document_line in document_lines)
                {
                    if (document_line.document_header_id == id)
                    {
                        product_stock product_stock = _context.product_stock.FirstOrDefault(ps => ps.product_id == document_line.product_id);

                        // Поле «количество»
                        product_stock.actual_quantity -= document_line.quantity;
                        // Поле «количество в резерве»
                        product_stock.reserved_quantity -= document_line.quantity;
                        // Если любая из величин в таблице складских остатков оказывается меньше нуля, то происходит полная отмена операции 
                        if (product_stock.actual_quantity < 0 || product_stock.reserved_quantity < 0)
                        {
                            return RedirectToAction("Index");
                        }

                        product_stocks.Add(product_stock);
                    }
                }

                // Обновляем значения всех складских остатков
                foreach (var product_stock in product_stocks)
                {
                    _context.product_stock.Update(product_stock);
                    _context.SaveChanges();
                }

                // Обновляем состояние документа на "Списано"
                document_header.document_status = "Списано";

                _context.document_header.Update(document_header);
                _context.SaveChanges();

                // Перенаправляем на метод Index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для отмены списания документа по id
        [HttpGet("cancelWritingOffDocument/{id}")]
        public IActionResult CancelWritingOffDocument(int id)
        {
            try
            {
                // Находим заголовок документа по id
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == id);

                // Получаем список всех строк документа
                List<document_line> document_lines = _context.document_line.ToList();

                // Для каждой строчки в документе на величину поля «количество» увеличиваем величину поля «количество» в таблице складских остатков
                foreach (var document_line in document_lines)
                {
                    if (document_line.document_header_id == id)
                    {
                        product_stock product_stock = _context.product_stock.FirstOrDefault(ps => ps.product_id == document_line.product_id);

                        product_stock.actual_quantity += document_line.quantity;

                        _context.product_stock.Update(product_stock);
                        _context.SaveChanges();
                    }
                }

                // Обновляем состояние документа на "Черновик"
                document_header.document_status = "Черновик";

                _context.document_header.Update(document_header);
                _context.SaveChanges();

                // Перенаправляем на метод Index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }
    }
}
