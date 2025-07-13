using EdgeDB;
using Microsoft.Extensions.Logging;

namespace Reserve.Core.Features.Queue;

public class QueueRepository : IQueueRepository
{
    private readonly EdgeDBClient _client;
    private readonly ILogger<QueueRepository> _logger;

    public QueueRepository(EdgeDBClient client, ILogger<QueueRepository> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<QueueEvent?> Create(QueueEvent? queueEvent)
    {
        try
        {
            var query = @"WITH Inserted := (
                INSERT QueueEvent {
                    title := <str>$title,
                    organizer_email := <str>$organizer_email,
                    description := <str>$description,
                    current_number_served := <int32>$current_number_served,
                    ticket_counter := <int32>$ticket_counter,
                    last_reset := datetime_current()
                }
              )
              SELECT Inserted{*};";

            var result = await _client.QuerySingleAsync<QueueEvent>(query, new Dictionary<string, object?>
        {
            {"title", queueEvent?.Title },
            {"organizer_email", queueEvent?.OrganizerEmail },
            {"description", queueEvent?.Description },
            {"current_number_served", queueEvent?.CurrentNumberServed },
            {"ticket_counter", queueEvent?.TicketCounter }
        });

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }

    public async Task<QueueEvent?> GetQueueEventByID(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        try
        {
            Guid guidId = Guid.Parse(id);
            var query = @"SELECT QueueEvent {*} FILTER .id = <uuid>$id;";
            var results = await _client.QueryAsync<QueueEvent>(query, new Dictionary<string, object?>
        {
            {"id", guidId }
        });
            var result = results.FirstOrDefault();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }

    public async Task<QueueTicket?> GetQueueTicketByID(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);
        try
        {
            Guid guidId = Guid.Parse(id);
            var query = @"SELECT QueueTicket {*} FILTER .id = <uuid>$id;";
            var results = await _client.QueryAsync<QueueTicket>(query, new Dictionary<string, object?>
        {
            {"id", guidId }
        });
            var result = results.FirstOrDefault();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }

    public async Task<int> GetNextQueueNumber(string queueId)
    {
        ArgumentException.ThrowIfNullOrEmpty(queueId);
        try
        {
            Guid guidId = Guid.Parse(queueId);
            var query = @"SELECT QueueEvent.ticket_counter + 1
                    FILTER QueueEvent.id = <uuid>$id;";
            var results = await _client.QueryAsync<int>(query, new Dictionary<string, object?>
    {
        {"id", guidId }
    });
            var result = results.FirstOrDefault();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return 0;
        }
    }

    public async Task IncrementTicketCounter(string queueId)
    {
        try
        {
            Guid guidId = Guid.Parse(queueId);
            var query = @"UPDATE QueueEvent
                  FILTER .id = <uuid>$id
                  SET { ticket_counter := .ticket_counter + 1 };";
            await _client.ExecuteAsync(query, new Dictionary<string, object?>
    {
        {"id", guidId }
    });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public async Task<QueueTicket?> RegisterCustomer(QueueTicket? queueTicket)
    {
        ArgumentNullException.ThrowIfNull(queueTicket);

        var queueEvent = queueTicket.QueueEvent ?? throw new ArgumentNullException(nameof(queueTicket.QueueEvent), "QueueEvent cannot be null");
        Guid guidId = queueEvent.Id;

        try
        {
            var query = @"WITH Inserted := (
            INSERT QueueTicket {
                customer_name := <str>$customer_name,
                customer_phone_number := <str>$customer_phone_number,
                queue_number := <int32>$queue_number,
                queue_event := (SELECT QueueEvent FILTER .id = <uuid>$queueId)
            }
          )
          SELECT Inserted {
              id,
              customer_name,
              customer_phone_number,
              queue_number,
              queue_event: {*}
          };";

            var result = await _client.QuerySingleAsync<QueueTicket>(query, new Dictionary<string, object?>
        {
            {"customer_name", queueTicket.CustomerName },
            {"customer_phone_number", queueTicket.CustomerPhoneNumber },
            {"queue_number", queueTicket.QueueNumber },
            {"queueId", guidId }
        });
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
    }


    public async Task ServeCurrentCustomer(string queueEventId)
    {
        ArgumentException.ThrowIfNullOrEmpty(queueEventId);
        try
        {
            Guid guidId = Guid.Parse(queueEventId);
            var query = @"
                WITH
                QueueEvent := (SELECT QueueEvent FILTER .id = <uuid>$queueEventId),
                CurrentTicket := (SELECT QueueTicket FILTER .queue_event = QueueEvent AND .queue_number = QueueEvent.current_number_served)
                DELETE QueueTicket FILTER .id = CurrentTicket.id;
            ";
            await _client.ExecuteAsync(query, new Dictionary<string, object?>
        {
            {"queueEventId", guidId }
        });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
    public async Task<List<QueueTicket?>> GetAttendees(string queueEventId)
    {
        ArgumentException.ThrowIfNullOrEmpty(queueEventId);
        try
        {
            Guid guidId = Guid.Parse(queueEventId);
            var query = @"
               WITH
                QueueEvent := (SELECT QueueEvent FILTER .id = <uuid>$queueEventId)
            SELECT QueueTicket {
                id,
                customer_name,
                customer_phone_number,
                queue_number,
                status,
                queue_event: {*}
            }
            FILTER .queue_event = QueueEvent AND NOT exists .status;
            ";
            var result = await _client.QueryAsync<QueueTicket>(query, new Dictionary<string, object?>
{
    {"queueEventId", guidId }
});
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return [];
        }
    }

    public async Task MarkAsReserved(QueueTicket? ticket, int queueNumber)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        ArgumentNullException.ThrowIfNull(ticket.QueueEvent); 
        ArgumentNullException.ThrowIfNull(queueNumber);

        try
        {
            var query = @"WITH Updated := (
            UPDATE QueueEvent
            FILTER .id = <uuid>$eventId
            SET {
                    current_number_served := .current_number_served + 1
                }
            ),
            UpdatedTicket := (
                UPDATE QueueTicket
                FILTER .queue_number = <int32>$queueNumber AND .queue_event.id = <uuid>$eventId
                SET {
                    status := 'served'
                }
            )
            SELECT Updated {
                id,
                title,
                organizer_email,
                description,
                current_number_served,
                ticket_counter
            };";

            var result = await _client.QuerySingleAsync<QueueEvent>(query, new Dictionary<string, object?>
        {
            {"queueNumber", queueNumber },
            {"eventId", ticket.QueueEvent.Id }
        });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }


    public async Task ResetQueue(string queueEventId)
    {
        ArgumentException.ThrowIfNullOrEmpty(queueEventId);
        try
        {
            Guid guidId = Guid.Parse(queueEventId);
            var updatequery = @"    UPDATE QueueEvent
                  FILTER .id = <uuid>$eventId
                  SET {
                      current_number_served := 1,
                      ticket_counter := 0,
                      last_reset:= datetime_current()
                  };";

            await _client.QueryAsync<QueueEvent>(updatequery, new Dictionary<string, object?>
      {
          {"eventId", guidId}
      });

            var updateTicketQuery = @"    UPDATE QueueTicket
         FILTER .queue_event.id = <uuid>$eventId
         SET {
             status := 'reset'
         };";

            await _client.QueryAsync<QueueEvent>(updateTicketQuery, new Dictionary<string, object?>
      {
          {"eventId", guidId}
      });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public async Task<bool> DoesPhoneNumberExist(string phoneNumber, string queueEventId)
    {
        ArgumentException.ThrowIfNullOrEmpty(phoneNumber);
        ArgumentException.ThrowIfNullOrEmpty(queueEventId);
        try
        {
            Guid guidId = Guid.Parse(queueEventId);
            var query = @"SELECT EXISTS (
                    SELECT QueueTicket
                    FILTER .customer_phone_number = <str>$phoneNumber AND .queue_event.id = <uuid>$queueEventId
                  );";
            var result = await _client.QuerySingleAsync<bool>(query, new Dictionary<string, object?>
    {
        {"phoneNumber", phoneNumber },
        {"queueEventId", guidId }
    });
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }
    }
}