
"В каталоге мы создаём два новых атрибута, которые называм "цена в USD" и "цена в euro"
У нужных товаров прописываем нужную цену в одном из этих атрибутов.

Приложение, которое раз в сутки запрашивает у ecwid-based сайта всю информацию о USD и Euro ценах у товаров. Если у каких то товаров в этих атрибутах пусто, 
то приложение с ними ничего неделает.
Далее приложение обращается в какой-нибудь сервис конвертации валют. Получает от него сегодняшний курс.
Исходя из полученных данных наше приложение считает актуальную цену каждого товара в рублях и обновляет цену в рублях у каждого товара в магазине 
(но только у тех, где в атрибутах есть цена в долларах или евро). И так каждый день.
приложенька на нашем сервере должна работать" ─ со слов заказчика
